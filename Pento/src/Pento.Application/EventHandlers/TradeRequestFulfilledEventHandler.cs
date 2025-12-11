using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers;

internal sealed class TradeRequestFulfilledEventHandler(
    INotificationService notificationService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<User> userRepository)
    : DomainEventHandler<TradeRequestFulfilledDomainEvent>
{
    public override async Task Handle(
        TradeRequestFulfilledDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeRequestFulfilledEventHandler), TradeErrors.RequestNotFound);
        }
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(request.TradeOfferId, cancellationToken);
        if (offer == null) 
        {
            throw new PentoException(nameof(TradeRequestFulfilledEventHandler), TradeErrors.OfferNotFound);
        }
        User? offerUser = await userRepository.GetByIdAsync(offer.UserId, cancellationToken);
        if (offerUser == null)
        {
            throw new PentoException(nameof(TradeRequestFulfilledEventHandler), UserErrors.NotFound);
        }
        string title = "TradeAway Fulfilled";
        string body = $"Your trade with {offerUser.FirstName} has been fulfilled.";
        var payload = new Dictionary<string, string>
        {
            { "tradeRequestId", request.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToUserAsync(
            request.UserId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeRequestFulfilledEventHandler), notificationResult.Error);
        }
        await hubContext.Clients.All
            .TradeRequestFulfilled(request.Id);
    }
}
