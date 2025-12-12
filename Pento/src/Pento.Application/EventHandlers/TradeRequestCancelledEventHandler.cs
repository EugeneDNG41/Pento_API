using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Application.EventHandlers;

internal sealed class TradeRequestCancelledEventHandler(
    IConverterService converterService,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<TradeRequestCancelledDomainEvent>
{
    public override async Task Handle(
        TradeRequestCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeRequestCancelledEventHandler), TradeErrors.RequestNotFound);
        }
        IEnumerable<TradeItemRequest> tradeItemRequests = await tradeItemRequestRepository.FindAsync(
            tir => tir.RequestId == request.Id,
            cancellationToken);
        var foodItemIds = tradeItemRequests.Select(tir => tir.FoodItemId).Distinct().ToList();
        IEnumerable<FoodItem> foodItems = await foodItemRepository.FindAsync(
            fi => foodItemIds.Contains(fi.Id),
            cancellationToken);
        foreach (TradeItemRequest tradeItemRequest in tradeItemRequests)
        {
            FoodItem foodItem = foodItems.Single(fi => fi.Id == tradeItemRequest.FoodItemId);
            Result<decimal> conversionResult = await converterService.ConvertAsync(
                tradeItemRequest.Quantity,
                tradeItemRequest.UnitId,
                foodItem.UnitId,
                cancellationToken);
            if (conversionResult.IsFailure)
            {
                throw new PentoException(nameof(TradeRequestCancelledEventHandler), conversionResult.Error);
            }
            foodItem.AdjustReservedQuantity(conversionResult.Value);
        }
        foodItemRepository.UpdateRange(foodItems);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
