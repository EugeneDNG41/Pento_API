using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Users.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Sessions.SendMessage;

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
