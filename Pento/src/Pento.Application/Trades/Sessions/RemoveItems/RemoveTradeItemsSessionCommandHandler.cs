using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Sessions.RemoveItems;

internal sealed class RemoveTradeItemsSessionCommandHandler(
    IUserContext userContext,
    TradeService tradeService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeItemSession> tradeItemSessionRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork)
    : ICommandHandler<RemoveTradeSessionItemsCommand>
{
    public async Task<Result> Handle(RemoveTradeSessionItemsCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        Guid userId = userContext.UserId;
        if (householdId is null)
        {
            return Result.Failure(HouseholdErrors.NotInAnyHouseHold);
        }
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(command.TradeSessionId, cancellationToken);
        if (session == null) 
        {
            return Result.Failure(TradeErrors.SessionNotFound);
        }
        if (session.OfferUserId != userId && session.RequestUserId != userId)
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
        var restoredItems = new Dictionary<Guid, decimal>();
        foreach (TradeItemSession sessionItem in items)
        {
            if (sessionItem.ItemFrom == TradeItemSessionFrom.Offer && session.OfferUserId != userId
                || sessionItem.ItemFrom == TradeItemSessionFrom.Request && session.RequestUserId != userId)
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
            Result reconciliationResult = await tradeService.ReconcileRemovedTradeItemsDuringSessionAsync(
                session,
                sessionItem,
                foodItem,
                userId,
                cancellationToken);
            if (reconciliationResult.IsFailure)
            {
                return reconciliationResult;
            }
            tradeItemSessionRepository.Remove(sessionItem);
            restoredItems.Add(foodItem.Id, foodItem.Quantity);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(session.Id.ToString())
            .TradeSessionItemsRemoved(session.Id, command.TradeItemIds);
        foreach (KeyValuePair<Guid, decimal> restoredItem in restoredItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .FoodItemQuantityUpdated(restoredItem.Key, restoredItem.Value);
        }
        return Result.Success();
    }
} 
