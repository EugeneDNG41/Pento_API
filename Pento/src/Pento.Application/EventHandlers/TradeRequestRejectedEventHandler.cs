using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Notifications;
using Pento.Domain.Trades;

namespace Pento.Application.EventHandlers;

internal sealed class TradeRequestRejectedEventHandler(
    IConverterService converterService,
    INotificationService notificationService,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<Household> householdRepository,
    IUnitOfWork unitOfWork)
     : DomainEventHandler<TradeRequestRejectedDomainEvent>
{
    public async override Task Handle(TradeRequestRejectedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeRequestRejectedEventHandler), TradeErrors.RequestNotFound);
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
        await foodItemRepository.UpdateRangeAsync(foodItems, cancellationToken);
        
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(request.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeRequestRejectedEventHandler), TradeErrors.OfferNotFound);
        }
        Household? offerHousehold = await householdRepository.GetByIdAsync(offer.HouseholdId, cancellationToken);
        if (offerHousehold == null)
        {
            throw new PentoException(nameof(TradeRequestRejectedEventHandler), HouseholdErrors.NotFound);
        }
        string title = "Trade Request Rejected";
        string body = $"Household {offerHousehold.Name} has rejected your trade request.";
        var payload = new Dictionary<string, string>
            {
                { "tradeRequestId", request.Id.ToString() },
                { "tradeOfferId", offer.Id.ToString() }
            };
        Result notificationResult = await notificationService.SendToHouseholdAsync(
            request.HouseholdId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new PentoException(nameof(TradeRequestRejectedEventHandler), notificationResult.Error);
        }
    }
}
