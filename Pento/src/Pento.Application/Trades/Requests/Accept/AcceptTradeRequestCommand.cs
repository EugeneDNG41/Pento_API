using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Users.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Requests.Accept;

public sealed record AcceptTradeRequestCommand(Guid OfferId, Guid RequestId) : ICommand<Guid>;

internal sealed class AcceptTradeRequestCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeItemOffer> tradeItemOfferRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<TradeItemSession> tradeItemSessionRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<AcceptTradeRequestCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AcceptTradeRequestCommand command, CancellationToken cancellationToken)
    {
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(command.OfferId, cancellationToken);
        if (offer == null)
        {
            return Result.Failure<Guid>(TradeErrors.OfferNotFound);
        }
        if (offer.UserId != userContext.UserId)
        {
            return Result.Failure<Guid>(TradeErrors.OfferForbiddenAccess);
        }
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(command.RequestId, cancellationToken);
        if (request == null)
        {
            return Result.Failure<Guid>(TradeErrors.RequestNotFound);
        }
        if (offer.Status != TradeStatus.Open)
        {
            return Result.Failure<Guid>(TradeErrors.InvalidOfferState);
        }
        if (request.Status != TradeRequestStatus.Pending)
        {
            return Result.Failure<Guid>(TradeErrors.InvalidRequestState);
        }
        if (offer.UserId == request.UserId)
        {
            return Result.Failure<Guid>(TradeErrors.CannotTradeWithSelf);
        }
        if (offer.HouseholdId == request.HouseholdId)
        {
            return Result.Failure<Guid>(TradeErrors.CannotTradeWithinHousehold);
        }
        var session = TradeSession.Create(offer.Id, request.Id, offer.UserId, request.UserId, dateTimeProvider.UtcNow);
        tradeSessionRepository.Add(session);
        IEnumerable<TradeItemOffer> offerItems = await tradeItemOfferRepository.FindAsync(
            item => item.OfferId == session.TradeOfferId,
            cancellationToken);
        IEnumerable<TradeItemRequest> requestItems = await tradeItemRequestRepository.FindAsync(
            item => item.RequestId == session.TradeRequestId,
            cancellationToken);
        var sessionItems = new List<TradeItemSession>();
        foreach (TradeItemOffer offerItem in offerItems)
        {
            var sessionItem = TradeItemSession.Create(
                offerItem.FoodItemId,
                offerItem.Quantity,
                offerItem.UnitId,
                session.Id,
                TradeItemSessionFrom.Offer);
            sessionItems.Add(sessionItem);
        }
        foreach (TradeItemRequest requestItem in requestItems)
        {
            var sessionItem = TradeItemSession.Create(
                requestItem.FoodItemId,
                requestItem.Quantity,
                requestItem.UnitId,
                session.Id,
                TradeItemSessionFrom.Request);
            sessionItems.Add(sessionItem);
        }
        tradeItemSessionRepository.AddRange(sessionItems);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return session.Id;
    }
}

public sealed record CancelTradeSessionCommand(Guid TradeSessionId) : ICommand;

internal sealed class CancelTradeSessionCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CancelTradeSessionCommand>
{
    public async Task<Result> Handle(CancelTradeSessionCommand command, CancellationToken cancellationToken)
    {
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null)
        {
            return Result.Failure(TradeErrors.SessionNotFound);
        }
        if (session.OfferUserId != userContext.UserId && session.RequestUserId != userContext.UserId)
        {
            return Result.Failure(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure(TradeErrors.InvalidSessionState);
        }     
        session.Cancel(); //cancel request as well?
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(session.Id.ToString())
            .TradeSessionCancelled(session.Id);
        return Result.Success();
    }
}

public sealed record ConfirmTradeSessionCommand(Guid TradeSessionId) : ICommand; //need to fulfill trade items, perhaps in event handler, and confirm by both
internal sealed class CompleteTradeSessionCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<ConfirmTradeSessionCommand>
{
    public async Task<Result> Handle(ConfirmTradeSessionCommand command, CancellationToken cancellationToken)
    {
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null)
        {
            return Result.Failure(TradeErrors.SessionNotFound);
        }
        if (session.OfferUserId != userContext.UserId && session.RequestUserId != userContext.UserId)
        {
            return Result.Failure(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure(TradeErrors.InvalidSessionState);
        }
        if (session.OfferUserId == userContext.UserId)
        {
            session.ConfirmByOfferUser();
        }
        else if (session.RequestUserId == userContext.UserId)
        {
            session.ConfirmByRequestUser();
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
public sealed record SendTradeMessageCommand(Guid TradeSessionId, string Message) : ICommand<TradeMessageResponse>;
internal sealed class SendTradeMessageCommandValidator : AbstractValidator<SendTradeMessageCommand>
{
    public SendTradeMessageCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade Session Id is required.");
        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Message must be between 1 and 500 characters."); //business rule
    }
}
public sealed record TradeMessageResponse(Guid TradeMessageId, Guid TradeSessionId, string MessageText, DateTime SentOn, BasicUserResponse Sender);
internal sealed class SendTradeMessageCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
    IGenericRepository<User> userRepository,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionMessage> tradeSessionMessageRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<SendTradeMessageCommand, TradeMessageResponse>
{
    public async Task<Result<TradeMessageResponse>> Handle(SendTradeMessageCommand command, CancellationToken cancellationToken)
    {
        User? user =  await userRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<TradeMessageResponse>(UserErrors.NotFound);
        }
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null)
        {
            return Result.Failure<TradeMessageResponse>(TradeErrors.SessionNotFound);
        }
        if (session.OfferUserId != userContext.UserId && session.RequestUserId != userContext.UserId)
        {
            return Result.Failure<TradeMessageResponse>(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure<TradeMessageResponse>(TradeErrors.InvalidSessionState);
        }
        var message = TradeSessionMessage.Create(
            session.Id,
            userContext.UserId,
            command.Message,
            dateTimeProvider.UtcNow);
        tradeSessionMessageRepository.Add(message);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var response = new TradeMessageResponse(
            message.Id,
            session.Id,
            message.MessageText,
            message.SentOn,
            new BasicUserResponse(user.Id, user.FirstName, user.LastName, user.AvatarUrl));
        await hubContext.Clients.Group(session.Id.ToString())
            .TradeMessageSent(response);
        return response;
    }
}
public sealed record RemoveTradeSessionItemsCommand(Guid TradeSessionId, Guid[] TradeItemIds) : ICommand;
internal sealed class RemoveTradeSessionItemsCommandValidator : AbstractValidator<RemoveTradeSessionItemsCommand>
{
    public RemoveTradeSessionItemsCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade session Id is required.");
        RuleFor(x => x.TradeItemIds)
            .NotNull().WithMessage("Trade item Ids must not be null.")
            .Must(ids => ids.Length > 0).WithMessage("At least one trade item ID must be provided.");
        RuleForEach(x => x.TradeItemIds)
            .NotEmpty().WithMessage("Trade item Ids must not contain empty GUIDs.");
    }
}
internal sealed class RemoveTradeItemsSessionCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeItemSession> tradeItemSessionRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveTradeSessionItemsCommand>
{
    public async Task<Result> Handle(RemoveTradeSessionItemsCommand command, CancellationToken cancellationToken)
    {
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null) 
        {
            return Result.Failure(TradeErrors.SessionNotFound);
        }
        if (session.OfferUserId != userContext.UserId && session.RequestUserId != userContext.UserId)
        {
            return Result.Failure(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure(TradeErrors.InvalidSessionState);
        }
        IEnumerable<TradeItemSession> items = await tradeItemSessionRepository.FindAsync(
            item => item.SessionId == command.TradeSessionId
                && command.TradeItemIds.Contains(item.Id),
            cancellationToken);
        foreach (TradeItemSession item in items)
        {
            if (item.ItemFrom == TradeItemSessionFrom.Offer && session.OfferUserId != userContext.UserId 
                || item.ItemFrom == TradeItemSessionFrom.Request && session.RequestUserId != userContext.UserId)
            {
                return Result.Failure(TradeErrors.ItemForbiddenAccess);
            }
            tradeItemSessionRepository.Remove(item);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(session.Id.ToString())
            .TradeSessionItemsRemoved(session.Id, command.TradeItemIds);
        return Result.Success();
    }
} 
