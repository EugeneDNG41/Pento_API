using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.RemoveItems;

internal sealed class RemoveTradeOfferItemsCommandHandler(
    IUserContext userContext,
    IConverterService converterService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeItemOffer> tradeItemRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveTradeOfferItemsCommand>
{
    public async Task<Result> Handle(RemoveTradeOfferItemsCommand command, CancellationToken cancellationToken)
    {
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
        IEnumerable<TradeItemOffer> items = await tradeItemRepository.FindAsync(
            item => item.OfferId == command.OfferId
                && command.TradeItemIds.Contains(item.Id),
            cancellationToken);
        var restoredItems = new Dictionary<Guid, decimal>();
        foreach (TradeItemOffer item in items)
        {
            FoodItem? foodItem = await foodItemRepository.GetByIdAsync(item.FoodItemId, cancellationToken);
            if (foodItem == null)
            {
                return Result.Failure(FoodItemErrors.NotFound);
            }
            if (foodItem.HouseholdId != householdId)
            {
                return Result.Failure(FoodItemErrors.ForbiddenAccess);
            }
            Result<decimal> restoredResult = await converterService
                .ConvertAsync(
                    item.Quantity,
                    item.UnitId,
                    foodItem.UnitId,
                    cancellationToken);
            if (restoredResult.IsFailure)
            {
                return Result.Failure(restoredResult.Error);
            }
            foodItem.AdjustReservedQuantity(restoredResult.Value);
            await foodItemRepository.UpdateAsync(foodItem, cancellationToken);
            await tradeItemRepository.RemoveAsync(item, cancellationToken);
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
