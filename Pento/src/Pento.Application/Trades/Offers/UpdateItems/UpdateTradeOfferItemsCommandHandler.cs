using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.UpdateItems;

internal sealed class UpdateTradeOfferItemsCommandHandler(
    IUserContext userContext,
    ITradeService tradeService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeItemOffer> tradeItemRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateTradeOfferItemsCommand>
{
    public async Task<Result> Handle(UpdateTradeOfferItemsCommand command, CancellationToken cancellationToken)
    {
        // Implementation would be similar to UpdateTradeRequestItemCommandHandler
        Guid? householdId = userContext.HouseholdId;
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(command.OfferId, cancellationToken);
        if (offer == null)
        {
            return Result.Failure(TradeErrors.OfferNotFound);
        }
        if (offer.HouseholdId != householdId)
        {
            return Result.Failure(TradeErrors.OfferForbiddenAccess);
        }
        if (offer.Status != TradeOfferStatus.Open)
        {
            return Result.Failure(TradeErrors.InvalidOfferState);
        }
        var affectedFoodItems = new Dictionary<Guid, decimal>();
        var affectedTradeItems = new List<TradeItemOffer>();
        foreach (UpdateTradeItemDto dto in command.Items)
        {
            TradeItemOffer? item = await tradeItemRepository.GetByIdAsync(dto.TradeItemId, cancellationToken);
            if (item is null)
            {
                return Result.Failure(TradeErrors.NotFound);
            }
            if (item.OfferId != offer.Id)
            {
                return Result.Failure(TradeErrors.ItemForbiddenAccess);
            }
            item.Update(dto.Quantity, dto.UnitId);
            FoodItem? foodItem = await foodItemRepository.GetByIdAsync(item.FoodItemId, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure(FoodItemErrors.NotFound);
            }
            if (foodItem.HouseholdId != householdId.Value)
            {
                return Result.Failure(FoodItemErrors.ForbiddenAccess);
            }
            Result reconciliationResult = await tradeService
                .ReconcileTradeItemsAddedOrUpdatedOutsideSessionAsync(offer.Id, null, userContext.UserId, item, foodItem, cancellationToken);
            if (reconciliationResult.IsFailure)
            {
                return Result.Failure(reconciliationResult.Error);
            }
            affectedFoodItems.Add(foodItem.Id, foodItem.Quantity);
            affectedTradeItems.Add(item);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (TradeItemOffer item in affectedTradeItems)
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
