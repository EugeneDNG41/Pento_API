using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Notifications;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers.Trades;

internal sealed class TradeOfferFulfilledEventHandler(
    INotificationService notificationService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<Household> householdRepository)
    : DomainEventHandler<TradeOfferFulfilledDomainEvent>
{
    public override async Task Handle(
        TradeOfferFulfilledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(domainEvent.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), TradeErrors.OfferNotFound);
        }
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null) 
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), TradeErrors.RequestNotFound);
        }
        Household? requestHousehold = await householdRepository.GetByIdAsync(offer.HouseholdId, cancellationToken);
        if (requestHousehold == null)
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), HouseholdErrors.NotFound);
        }
        string title = "Trade Fulfilled";
        string body = $"Your trade with household {requestHousehold.Name} has been fulfilled.";
        var payload = new Dictionary<string, string>
        {
            { "tradeOfferId", offer.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToHouseholdAsync(
            offer.HouseholdId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeOfferFulfilledEventHandler), notificationResult.Error);
        }
        await hubContext.Clients.All
            .TradeOfferFulfilled(offer.Id);
    }
}
