using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.Trades.Sessions.AddItems;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.Abstractions.Services;

public sealed class TradeService(
    IConverterService converter,
    IGenericRepository<TradeItemOffer> tradeItemOfferRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<FoodItem> foodItemRepository)
{
    public async Task<Result> ReconcileUpdatedTradeItemsDuringSessionAsync(
        TradeSession session, 
        TradeSessionItem sessionItem,
        FoodItem foodItem, 
        decimal newQuantity,
        Guid userId,        
        CancellationToken cancellationToken)
    {
        Result<decimal> currentQtyInItemUnit = await converter.ConvertAsync(
                    sessionItem.Quantity,
                    sessionItem.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
        if (currentQtyInItemUnit.IsFailure)
        {
            return Result.Failure(currentQtyInItemUnit.Error);
        }
        Result<decimal> newQtyInItemUnit = await converter.ConvertAsync(
                    newQuantity,
                    sessionItem.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
        if (newQtyInItemUnit.IsFailure)
        {
            return Result.Failure(newQtyInItemUnit.Error);
        }
        if (sessionItem.From == TradeItemFrom.Offer)
        {
            TradeItemOffer? originalOffer = (await tradeItemOfferRepository.FindAsync(
                tio => tio.FoodItemId == sessionItem.FoodItemId
                    && tio.OfferId == session.TradeOfferId,
                cancellationToken)).SingleOrDefault();
            Result<decimal>? originalQtyInItemUnit = originalOffer != null ? await converter.ConvertAsync(
                    originalOffer.Quantity,
                    originalOffer.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                ) : null;
            if (originalQtyInItemUnit != null && originalQtyInItemUnit.IsFailure)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(originalQtyInItemUnit.Error);
            }
            if (originalOffer == null)
            {
                foodItem.AdjustReservedQuantity(currentQtyInItemUnit.Value, userId);
                foodItemRepository.Update(foodItem);
            }
            else
            {              
                decimal qtyDifference = originalQtyInItemUnit != null && originalQtyInItemUnit.Value >= newQtyInItemUnit.Value ?
                    currentQtyInItemUnit.Value - originalQtyInItemUnit.Value :
                    currentQtyInItemUnit.Value - newQtyInItemUnit.Value;
                if (qtyDifference > 0) // More was reserved in session than originally offered, so only adjust the difference then
                {
                    foodItem.AdjustReservedQuantity(qtyDifference, userId);
                }
            }
        }
        else
        {
            TradeItemRequest? originalRequest = (await tradeItemRequestRepository.FindAsync(
                tio => tio.FoodItemId == sessionItem.FoodItemId
                    && tio.RequestId == session.TradeRequestId,
                cancellationToken)).SingleOrDefault();
            if (originalRequest == null)
            {
                foodItem.AdjustReservedQuantity(currentQtyInItemUnit.Value, userId);
                foodItemRepository.Update(foodItem);
            }
            else
            {
                Result<decimal> qtySessionInItemUnit = await converter.ConvertAsync(
                    sessionItem.Quantity,
                    sessionItem.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
                if (currentQtyInItemUnit.IsFailure)
                {
                    return Result.Failure<IReadOnlyList<TradeItemResponse>>(currentQtyInItemUnit.Error);
                }
                Result<decimal> qtyRequestInItemUnit = await converter.ConvertAsync(
                    originalRequest.Quantity,
                    originalRequest.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
                if (qtyRequestInItemUnit.IsFailure)
                {
                    return Result.Failure<IReadOnlyList<TradeItemResponse>>(qtyRequestInItemUnit.Error);
                }
                decimal qtyDifference = qtySessionInItemUnit.Value - qtyRequestInItemUnit.Value;
                if (qtyDifference > 0) // More was reserved in session than originally requested, so only adjust the difference then
                {
                    foodItem.AdjustReservedQuantity(qtyDifference, userId);
                }
            }
        }
        foodItemRepository.Update(foodItem);
        return Result.Success();

    }
    public async Task<Result> ReconcileRemovedTradeItemsDuringSessionAsync(
        TradeSession session, 
        TradeSessionItem sessionItem,
        FoodItem foodItem, 
        Guid userId,        
        CancellationToken cancellationToken)
    {
        Result<decimal> currentQtyInItemUnit = await converter.ConvertAsync(
                    sessionItem.Quantity,
                    sessionItem.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
        if (currentQtyInItemUnit.IsFailure)
        {
            return Result.Failure(currentQtyInItemUnit.Error);
        }
        if (sessionItem.From == TradeItemFrom.Offer)
        {
            TradeItemOffer? originalOffer = (await tradeItemOfferRepository.FindAsync(
                tio => tio.FoodItemId == sessionItem.FoodItemId
                    && tio.OfferId == session.TradeOfferId,
                cancellationToken)).SingleOrDefault();
            if (originalOffer == null)
            {
                foodItem.AdjustReservedQuantity(currentQtyInItemUnit.Value, userId);
            }
            else
            {
                Result<decimal> qtyOfferInItemUnit = await converter.ConvertAsync(
                    originalOffer.Quantity,
                    originalOffer.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
                if (qtyOfferInItemUnit.IsFailure)
                {
                    return Result.Failure<IReadOnlyList<TradeItemResponse>>(qtyOfferInItemUnit.Error);
                }
                decimal qtyDifference = currentQtyInItemUnit.Value - qtyOfferInItemUnit.Value;
                if (qtyDifference > 0) // More was reserved in session than originally offered, so only adjust the difference then
                {
                    foodItem.AdjustReservedQuantity(qtyDifference, userId);
                }                                  
            }
        }
        else
        {
            TradeItemRequest? originalRequest = (await tradeItemRequestRepository.FindAsync(
                tio => tio.FoodItemId == sessionItem.FoodItemId
                    && tio.RequestId == session.TradeRequestId,
                cancellationToken)).SingleOrDefault();
            if (originalRequest == null)
            {
                foodItem.AdjustReservedQuantity(currentQtyInItemUnit.Value, userId);
            }
            else
            {
                Result<decimal> qtyRequestInItemUnit = await converter.ConvertAsync(
                    originalRequest.Quantity,
                    originalRequest.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
                if (qtyRequestInItemUnit.IsFailure)
                {
                    return Result.Failure<IReadOnlyList<TradeItemResponse>>(qtyRequestInItemUnit.Error);
                }
                decimal qtyDifference = currentQtyInItemUnit.Value - qtyRequestInItemUnit.Value;
                if (qtyDifference > 0) // More was reserved in session than originally requested, so only adjust the difference then
                {
                    foodItem.AdjustReservedQuantity(qtyDifference, userId);
                }
            }
        }
        foodItemRepository.Update(foodItem);
        return Result.Success();
    }
    public async Task<Result> ReconcileAddedTradeItemsDuringSessionAsync(
        TradeSession session,
        TradeSessionItem sessionItem,
        FoodItem foodItem,
        Guid userId,
        CancellationToken cancellationToken)
    {
        Result<decimal> currentQtyInItemUnit = await converter.ConvertAsync(
                    sessionItem.Quantity,
                    sessionItem.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
        if (currentQtyInItemUnit.IsFailure)
        {
            return Result.Failure(currentQtyInItemUnit.Error);
        }
        if (sessionItem.From == TradeItemFrom.Offer)
        {
            TradeItemOffer? originalOffer = (await tradeItemOfferRepository.FindAsync(
                tio => tio.FoodItemId == sessionItem.FoodItemId
                    && tio.OfferId == session.TradeOfferId,
                cancellationToken)).SingleOrDefault();
            if (originalOffer == null)
            {
                foodItem.Reserve(currentQtyInItemUnit.Value, sessionItem.Quantity, sessionItem.UnitId, userId);
            }
            else
            {
                Result<decimal> qtyOfferInItemUnit = await converter.ConvertAsync(
                    originalOffer.Quantity,
                    originalOffer.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
                if (qtyOfferInItemUnit.IsFailure)
                {
                    return Result.Failure<IReadOnlyList<TradeItemResponse>>(qtyOfferInItemUnit.Error);
                }
                decimal qtyDifference = currentQtyInItemUnit.Value - qtyOfferInItemUnit.Value;
                if (qtyDifference > 0)
                {
                    foodItem.Reserve(qtyDifference, sessionItem.Quantity, sessionItem.UnitId, userId);
                }
            }
        }
        else
        {
            TradeItemRequest? originalRequest = (await tradeItemRequestRepository.FindAsync(
                tio => tio.FoodItemId == sessionItem.FoodItemId
                    && tio.RequestId == session.TradeRequestId,
                cancellationToken)).SingleOrDefault();
            if (originalRequest == null)
            {
                foodItem.Reserve(currentQtyInItemUnit.Value, sessionItem.Quantity, sessionItem.UnitId, userId);
            }
            else
            {
                Result<decimal> qtyRequestInItemUnit = await converter.ConvertAsync(
                    originalRequest.Quantity,
                    originalRequest.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
                if (qtyRequestInItemUnit.IsFailure)
                {
                    return Result.Failure<IReadOnlyList<TradeItemResponse>>(qtyRequestInItemUnit.Error);
                }
                decimal qtyDifference = currentQtyInItemUnit.Value - qtyRequestInItemUnit.Value;
                if (qtyDifference > 0) // More was reserved in session than originally requested, so only adjust the difference then
                {
                    foodItem.Reserve(qtyDifference, sessionItem.Quantity, sessionItem.UnitId, userId);
                }
            }
        }
        foodItemRepository.Update(foodItem);
        return Result.Success();
    }
}
