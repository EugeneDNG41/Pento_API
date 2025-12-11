using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.UpdateItems;

internal sealed class UpdateTradeItemsSessionCommandHandler(
    IUserContext userContext,
    TradeService tradeService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeItemSession> tradeItemSessionRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateTradeSessionItemsCommand>
{
    public async Task<Result> Handle(UpdateTradeSessionItemsCommand command, CancellationToken cancellationToken)
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
        TradeItemSessionFrom from = session.OfferUserId == userId
                ? TradeItemSessionFrom.Offer
                : TradeItemSessionFrom.Request;
        
        var affectedFoodItems = new Dictionary<Guid, decimal>();
        var affectedTradeItems = new List<TradeItemSession>();
        foreach (UpdateTradeItemDto dto in command.Items)
        {
            TradeItemSession? sessionItem =
                await tradeItemSessionRepository.GetByIdAsync(dto.TradeItemId, cancellationToken);
            if (sessionItem is null)
            {
                return Result.Failure(TradeErrors.NotFound);
            }
            if (sessionItem.ItemFrom != from || sessionItem.SessionId != session.Id)
            {
                return Result.Failure(TradeErrors.ItemForbiddenAccess);
            }
            FoodItem? foodItem = 
                await foodItemRepository.GetByIdAsync(sessionItem.FoodItemId, cancellationToken);
            if (foodItem is null)
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
            sessionItem.Update(dto.Quantity, dto.UnitId);
            tradeItemSessionRepository.Update(sessionItem);

            affectedFoodItems.Add(foodItem.Id, foodItem.Quantity);
            affectedTradeItems.Add(sessionItem);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (TradeItemSession item in affectedTradeItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .TradeItemUpdated(item.Id, item.Quantity, item.UnitId);
        }
        foreach (KeyValuePair<Guid, decimal> affectedItem in affectedFoodItems)
        {
            await hubContext.Clients.Group(householdId.Value.ToString())
                .FoodItemQuantityUpdated(affectedItem.Key, affectedItem.Value);
        }
        return Result.Success();
    }
}
