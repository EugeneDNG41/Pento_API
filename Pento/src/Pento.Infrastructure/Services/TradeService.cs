using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Infrastructure.Services;

public sealed class TradeService(
    IConverterService converter,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionItem> tradeSessionItemRepository,
    IGenericRepository<TradeItemOffer> tradeItemOfferRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<FoodItem> foodItemRepository) : ITradeService
{
    public async Task<Result> ReconcileTradeItemsAddedOrUpdatedOutsideSessionAsync(
        Guid offerId,
        Guid? requestId,
        Guid userId,
        TradeItem tradeItem,
        FoodItem foodItem,
        CancellationToken cancellationToken)
    {
        Result<decimal> qtyInItemUnit = await converter.ConvertAsync(
                    tradeItem.Quantity, tradeItem.UnitId, foodItem.UnitId, cancellationToken);

        if (qtyInItemUnit.IsFailure)
        {
            return Result.Failure<Guid>(qtyInItemUnit.Error);
        }
        if (qtyInItemUnit.Value > foodItem.Quantity)
        {
            return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
        }
        decimal reservedQty = qtyInItemUnit.Value;
        IEnumerable<TradeSession> ongoingSessions = await tradeSessionRepository.FindAsync(
            ts => ts.TradeOfferId == offerId
                  && (requestId == null || ts.TradeRequestId == requestId)
                  && ts.Status == TradeSessionStatus.Ongoing,
            cancellationToken);
        foreach (TradeSession ongoingSession in ongoingSessions)
        {
            TradeSessionItem? similarItem = (await tradeSessionItemRepository.FindAsync(
                tsi => tsi.SessionId == ongoingSession.Id
                       && tsi.FoodItemId == tradeItem.FoodItemId
                       && tsi.From == TradeItemFrom.Request,
                cancellationToken)).SingleOrDefault();
            if (similarItem != null)
            {
                Result<decimal> sessionQtyInItemUnit = await converter.ConvertAsync(
                    similarItem.Quantity, similarItem.UnitId, foodItem.UnitId, cancellationToken);
                if (sessionQtyInItemUnit.IsFailure)
                {
                    return Result.Failure<Guid>(sessionQtyInItemUnit.Error);
                }
                if (sessionQtyInItemUnit.Value <= qtyInItemUnit.Value)
                {
                    reservedQty -= sessionQtyInItemUnit.Value;
                }
                else
                {
                    reservedQty = 0;
                    break;
                }
            }
        }
        foodItem.Reserve(reservedQty, tradeItem.Quantity, tradeItem.UnitId, userId);
        await foodItemRepository.UpdateAsync(foodItem, cancellationToken);
        return Result.Success();
    }
    public async Task<Result> ReconcileUpdatedTradeItemsDuringSessionAsync(
        TradeSession session,
        TradeSessionItem sessionItem,
        FoodItem foodItem,
        decimal newQuantity,
        Guid newUnitId,
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
                    newUnitId,
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
            decimal qtyDifference = originalQtyInItemUnit != null && originalQtyInItemUnit.Value >= newQtyInItemUnit.Value ?
                    currentQtyInItemUnit.Value - originalQtyInItemUnit.Value :
                    currentQtyInItemUnit.Value - newQtyInItemUnit.Value;
            if (originalOffer == null ||  qtyDifference > 0)
            {
                foodItem.AdjustReservedQuantity(qtyDifference);
            }

        }
        else
        {
            TradeItemRequest? originalRequest = (await tradeItemRequestRepository.FindAsync(
                tio => tio.FoodItemId == sessionItem.FoodItemId
                    && tio.RequestId == session.TradeRequestId,
                cancellationToken)).SingleOrDefault();
            Result<decimal>? originalQtyInItemUnit = originalRequest != null ? await converter.ConvertAsync(
                    originalRequest.Quantity,
                    originalRequest.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                ) : null;
            if (originalQtyInItemUnit != null && originalQtyInItemUnit.IsFailure)
            {
                return Result.Failure<IReadOnlyList<TradeItemResponse>>(originalQtyInItemUnit.Error);
            }
            decimal qtyDifference = originalQtyInItemUnit != null && originalQtyInItemUnit.Value >= newQtyInItemUnit.Value ?
                    currentQtyInItemUnit.Value - originalQtyInItemUnit.Value :
                    currentQtyInItemUnit.Value - newQtyInItemUnit.Value;
            if (originalRequest == null || qtyDifference > 0)
            {
                foodItem.AdjustReservedQuantity(qtyDifference);
            }
        }
        await foodItemRepository.UpdateAsync(foodItem, cancellationToken);
        return Result.Success();

    }
    public async Task<Result> ReconcileTradeItemsRemovedFromSessionAsync(
        TradeSession session,
        TradeSessionItem sessionItem,
        FoodItem foodItem,
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
                foodItem.AdjustReservedQuantity(currentQtyInItemUnit.Value);
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
                    foodItem.AdjustReservedQuantity(qtyDifference);
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
                foodItem.AdjustReservedQuantity(currentQtyInItemUnit.Value);
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
                    foodItem.AdjustReservedQuantity(qtyDifference);
                }
            }
        }
        await foodItemRepository.UpdateAsync(foodItem, cancellationToken);
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
        await foodItemRepository.UpdateAsync(foodItem, cancellationToken);
        return Result.Success();
    }
}
