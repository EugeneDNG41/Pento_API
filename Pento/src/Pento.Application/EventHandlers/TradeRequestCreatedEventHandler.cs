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

internal sealed class TradeRequestCreatedEventHandler(
    INotificationService notificationService,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<User> userRepository)
    : DomainEventHandler<TradeRequestCreatedDomainEvent>
{
    public override async Task Handle(
        TradeRequestCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeRequest? request = await tradeRequestRepository.GetByIdAsync(domainEvent.TradeRequestId, cancellationToken);
        if (request == null)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), TradeErrors.RequestNotFound);
        }
        TradeOffer? offer = await tradeOfferRepository.GetByIdAsync(request.TradeOfferId, cancellationToken);
        if (offer == null)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), TradeErrors.OfferNotFound);
        }
        User? requestUser = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (requestUser == null)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), UserErrors.NotFound);
        }
        string title = "New TradeAway Request";
        string body = $"you have received a new trade request from {requestUser.FirstName}.";
        var payload = new Dictionary<string, string>
        {
            { "tradeRequestId", request.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToUserAsync(
            offer.UserId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeRequestCreatedEventHandler), notificationResult.Error);
        }
        await hubContext.Clients.User(offer.UserId.ToString())
            .TradeRequestCreated(request.Id, offer.Id);
    }
}
