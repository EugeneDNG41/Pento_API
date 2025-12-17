using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.UpdateItems;

internal sealed class UpdateTradeSessionItemsCommandHandler(
    IUserContext userContext,
    ITradeService tradeService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionItem> tradeItemSessionRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateTradeSessionItemsCommand>
{
    public async Task<Result> Handle(UpdateTradeSessionItemsCommand command, CancellationToken cancellationToken)
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
        TradeItemFrom from = session.OfferHouseholdId == householdId
                ? TradeItemFrom.Offer
                : TradeItemFrom.Request;
        
        var affectedFoodItems = new Dictionary<Guid, decimal>();
        var affectedTradeItems = new List<TradeSessionItem>();
        foreach (UpdateTradeItemDto dto in command.Items)
        {
            TradeSessionItem? sessionItem =
                await tradeItemSessionRepository.GetByIdAsync(dto.TradeItemId, cancellationToken);
            if (sessionItem is null)
            {
                return Result.Failure(TradeErrors.NotFound);
            }
            if (sessionItem.From != from || sessionItem.SessionId != session.Id)
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
            Result reconciliationResult = await tradeService.ReconcileUpdatedTradeItemsDuringSessionAsync(
                session,
                sessionItem,
                foodItem,
                dto.Quantity,
                dto.UnitId,
                cancellationToken);
            if (reconciliationResult.IsFailure)
            {
                return reconciliationResult;
            }
            sessionItem.Update(dto.Quantity, dto.UnitId);
            await tradeItemSessionRepository.UpdateAsync(sessionItem, cancellationToken);

            affectedFoodItems.Add(foodItem.Id, foodItem.Quantity);
            affectedTradeItems.Add(sessionItem);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (TradeSessionItem item in affectedTradeItems)
        {
            await hubContext.Clients.Group(session.Id.ToString())
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
