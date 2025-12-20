using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers.Trades;

internal sealed class TradeSessionMessageCreatedEventHandler(
    INotificationService notificationService,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeSessionMessage> tradeSessionMessageRepository,
    IGenericRepository<User> userRepository)
    : DomainEventHandler<TradeSessionMessageCreatedDomainEvent>
{
    public override async Task Handle(
        TradeSessionMessageCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        TradeSessionMessage? message = await tradeSessionMessageRepository.GetByIdAsync(domainEvent.TradeSessionMessageId, cancellationToken);
        if (message == null)
        {
            throw new PentoException(nameof(TradeSessionMessageCreatedEventHandler), TradeErrors.MessageNotFound);
        }
        TradeSession? session = await tradeSessionRepository.GetByIdAsync(message.TradeSessionId, cancellationToken);
        if (session == null)
        {
            throw new PentoException(nameof(TradeSessionMessageCreatedEventHandler), TradeErrors.SessionNotFound);
        }      
        User? senderUser = await userRepository.GetByIdAsync(message.UserId, cancellationToken);
        if (senderUser == null)
        {
            throw new PentoException(nameof(TradeSessionMessageCreatedEventHandler), UserErrors.NotFound);
        }
        Guid recipientHouseholdId = session.OfferHouseholdId == senderUser.HouseholdId
            ? session.RequestHouseholdId
            : session.OfferHouseholdId;
        string title = $"New Message From {senderUser.FirstName}";
        string body = $"{senderUser.FirstName}: {message.MessageText}.";
        var payload = new Dictionary<string, string>
        {
            { "tradeSessionId", session.Id.ToString() },
            { "tradeSessionMessageId", message.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToHouseholdAsync(
            recipientHouseholdId,
            title,
            body,
            NotificationType.Trade,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(TradeSessionMessageCreatedEventHandler), notificationResult.Error);
        }
    }
}
