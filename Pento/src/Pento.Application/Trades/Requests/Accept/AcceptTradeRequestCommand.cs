using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Trades.Sessions.AddItems;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;

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
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionNotFound);
        }
        if (session.OfferUserId != userContext.UserId && session.RequestUserId != userContext.UserId)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.InvalidSessionState);
        }     
        session.Cancel(); //cancel request as well?
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(session.Id.ToString())
            .TradeSessionCancelled(session.Id);
        return Result.Success();
    }
}

public sealed record ConfirmTradeSessionCommand(Guid TradeSessionId) : ICommand; //need to fulfill trade Items, perhaps in event handler, and confirm by both
internal sealed class CompleteTradeSessionCommandHandler(// check
    IUserContext userContext,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<ConfirmTradeSessionCommand>
{
    public async Task<Result> Handle(ConfirmTradeSessionCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId == null)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionNotFound);
        }
        if (session.OfferUserId != userContext.UserId && session.RequestUserId != userContext.UserId)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.InvalidSessionState);
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
