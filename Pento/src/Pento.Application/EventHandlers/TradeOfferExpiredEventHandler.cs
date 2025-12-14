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

internal sealed class TradeOfferExpiredEventHandler(
    INotificationService notificationService,
    IConverterService converterService,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeItemOffer> tradeItemOfferRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Household> householdRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<TradeOfferExpiredDomainEvent>
{
    public override async Task Handle(
        TradeOfferExpiredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(domainEvent.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeOfferCancelledEventHandler), TradeErrors.OfferNotFound);
        }

        IEnumerable<TradeItemOffer> tradeItemOffers = await tradeItemOfferRepository.FindAsync(
            tir => tir.OfferId == offer.Id,
            cancellationToken);
        var foodItemIds = tradeItemOffers.Select(tir => tir.FoodItemId).Distinct().ToList();
        IEnumerable<FoodItem> foodItems = await foodItemRepository.FindAsync(
            fi => foodItemIds.Contains(fi.Id),
            cancellationToken);
        foreach (TradeItemOffer tradeItemOffer in tradeItemOffers)
        {
            FoodItem foodItem = foodItems.Single(fi => fi.Id == tradeItemOffer.FoodItemId);
            Result<decimal> conversionResult = await converterService.ConvertAsync(
                tradeItemOffer.Quantity,
                tradeItemOffer.UnitId,
                foodItem.UnitId,
                cancellationToken);
            if (conversionResult.IsFailure)
            {
                throw new PentoException(nameof(TradeRequestCancelledEventHandler), conversionResult.Error);
            }
            foodItem.AdjustReservedQuantity(conversionResult.Value);
        }
        foodItemRepository.UpdateRange(foodItems);
        IEnumerable<TradeRequest> requests = await tradeRequestRepository.FindAsync(
            r => r.TradeOfferId == offer.Id && r.Status == TradeRequestStatus.Pending,
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (TradeRequest request in requests)
        {
            request.Cancel();
        }
        tradeRequestRepository.UpdateRange(requests);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        Household? offerHousehold = await householdRepository.GetByIdAsync(offer.HouseholdId, cancellationToken);
        if (offerHousehold == null)
        {
            throw new PentoException(nameof(TradeOfferCancelledEventHandler), HouseholdErrors.NotFound);
        }
        string title = "Trade Offer Expired";
        string body = $"Household {offerHousehold.Name}'s offer has expired";
        var payload = new Dictionary<string, string>
            {
                { "tradeOfferId", offer.Id.ToString() }
            };
        foreach (TradeRequest request in requests)
        {
            payload.Add("tradeRequestId", request.Id.ToString());
            Result notificationResult = await notificationService.SendToHouseholdAsync(
                request.HouseholdId,
                title,
                body,
                NotificationType.Trade,
                payload,
                cancellationToken);
            if (notificationResult.IsFailure)
            {
                throw new PentoException(nameof(TradeOfferCancelledEventHandler), notificationResult.Error);
            }
        }
    }
}
