using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.Confirm;

internal sealed class ConfirmTradeSessionCommandHandler(
    IUserContext userContext,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionItem> tradeSessionItemRepository,
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
        if (session.OfferHouseholdId != userContext.HouseholdId && session.RequestHouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.InvalidSessionState);
        }
        if (session.OfferHouseholdId == userContext.HouseholdId)
        {
            if (session.ConfirmedByOfferUserId == null)
            {
                bool existingTradeItems = await tradeSessionItemRepository
                    .AnyAsync(x => x.SessionId == session.Id, cancellationToken);
                if (!existingTradeItems)
                {
                    return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.NoTradeItemsInSession);
                }
                session.ConfirmByOfferUser(userContext.UserId);
            }
            else
            {
                session.ConfirmByOfferUser(null);
            }
        }
        else if (session.RequestHouseholdId == userContext.HouseholdId)
        {
            if (session.ConfirmedByRequestUserId == null)
            {
                bool existingTradeItems = await tradeSessionItemRepository
                    .AnyAsync(x => x.SessionId == session.Id, cancellationToken);
                if (!existingTradeItems)
                {
                    return Result.Failure<IReadOnlyList<TradeItemResponse>>(TradeErrors.NoTradeItemsInSession);
                }
                session.ConfirmByRequestUser(userContext.UserId);
            }
            else
            {
                session.ConfirmByRequestUser(null);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        bool confirmdeByOfferer = session.ConfirmedByOfferUserId != null;
        bool confirmedByRequester = session.ConfirmedByRequestUserId != null;
        await hubContext.Clients.Group(session.Id.ToString())
            .TradeSessionConfirm(confirmdeByOfferer, confirmedByRequester);
        return Result.Success();
    }
}
