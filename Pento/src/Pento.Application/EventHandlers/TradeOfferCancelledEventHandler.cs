using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Notifications;
using Pento.Domain.Trades;

namespace Pento.Application.EventHandlers;

internal sealed class TradeOfferCancelledEventHandler(
    INotificationService notificationService,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<Household> householdRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<TradeOfferCancelledDomainEvent>
{
    public override async Task Handle(
        TradeOfferCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(domainEvent.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeOfferCancelledEventHandler), TradeErrors.OfferNotFound);
        }
        Household? offerHousehold = await householdRepository.GetByIdAsync(offer.HouseholdId, cancellationToken);
        if (offerHousehold == null)
        {
            throw new PentoException(nameof(TradeOfferCancelledEventHandler), HouseholdErrors.NotFound);
        }
        IEnumerable<TradeRequest> requests = await tradeRequestRepository.FindAsync(
            r => r.TradeOfferId == offer.Id && r.Status == TradeRequestStatus.Pending,
            cancellationToken);
        string title = "Trade Offer Cancelled";
        string body = $"Household {offerHousehold.Name} has cancelled their trade offer";
        var payload = new Dictionary<string, string>
            {
                { "tradeOfferId", offer.Id.ToString() }
            };
        foreach (TradeRequest request in requests)
        {
            request.Cancel();
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
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
