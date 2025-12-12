using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Sessions.RemoveItems;

internal sealed class RemoveTradeSessionItemsCommandHandler(
    IUserContext userContext,
    TradeService tradeService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionItem> tradeItemSessionRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveTradeSessionItemsCommand>
{
    public async Task<Result> Handle(RemoveTradeSessionItemsCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure(HouseholdErrors.NotInAnyHouseHold);
        }
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null) 
        {
            return Result.Failure(TradeErrors.SessionNotFound);
        }
        if (session.OfferHouseholdId != householdId && session.RequestHouseholdId != householdId)
        {
            return Result.Failure(TradeErrors.SessionForbiddenAccess);
        }
        if (session.Status != TradeSessionStatus.Ongoing)
        {
            return Result.Failure(TradeErrors.InvalidSessionState);
        }
        IEnumerable<TradeSessionItem> items = await tradeItemSessionRepository.FindAsync(
            item => item.SessionId == command.TradeSessionId
                && command.TradeItemIds.Contains(item.Id),
            cancellationToken);
        var restoredItems = new Dictionary<Guid, decimal>();
        foreach (TradeSessionItem sessionItem in items)
        {
            if (sessionItem.From == TradeItemFrom.Offer && session.OfferHouseholdId != householdId
                || sessionItem.From == TradeItemFrom.Request && session.RequestHouseholdId != householdId)
            {
                return Result.Failure(TradeErrors.ItemForbiddenAccess);
            }
            FoodItem? foodItem = await foodItemRepository.GetByIdAsync(sessionItem.FoodItemId, cancellationToken);
            if (foodItem == null)
            {
                return Result.Failure(FoodItemErrors.NotFound);
            }
            if (foodItem.HouseholdId != householdId)
            {
                return Result.Failure(FoodItemErrors.ForbiddenAccess);
            }
            Result reconciliationResult = await tradeService.ReconcileTradeItemsRemovedFromSessionAsync(
                session,
                sessionItem,
                foodItem,
                cancellationToken);
            if (reconciliationResult.IsFailure)
            {
                return reconciliationResult;
            }
            tradeItemSessionRepository.Remove(sessionItem);
            restoredItems.Add(foodItem.Id, foodItem.Quantity);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.All.TradeItemsRemoved(command.TradeItemIds);
        foreach (KeyValuePair<Guid, decimal> restoredItem in restoredItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .FoodItemQuantityUpdated(restoredItem.Key, restoredItem.Value);
        }
        return Result.Success();
    }
} 
