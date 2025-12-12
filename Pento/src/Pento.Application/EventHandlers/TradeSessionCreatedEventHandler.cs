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

namespace Pento.Application.EventHandlers;

internal sealed class TradeSessionCreatedEventHandler(
    INotificationService notificationService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<Household> householdRepository)
    : DomainEventHandler<TradeSessionCreatedDomainEvent>
{
    public override async Task Handle(
        TradeSessionCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(domainEvent.TradeSessionId, cancellationToken);
        if (session == null)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), TradeErrors.SessionNotFound);
        }
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(session.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), TradeErrors.OfferNotFound);
        }
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(session.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), TradeErrors.RequestNotFound);
        }
        Household? offerHousehold = await householdRepository.GetByIdAsync(offer.HouseholdId, cancellationToken);
        if (offerHousehold == null)
        {
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), HouseholdErrors.NotFound);
        }
        string title = "Trade Session Opened";
        string body = $"Household  {offerHousehold.Name} has opened a trade session for your request.";
        var payload = new Dictionary<string, string>
        {
            { "tradeSessionId", session.Id.ToString() }
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
            throw new PentoException(nameof(TradeSessionCreatedEventHandler), notificationResult.Error);
        }      
    }
}
