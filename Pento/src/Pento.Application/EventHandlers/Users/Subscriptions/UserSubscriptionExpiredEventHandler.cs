using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;
using Pento.Domain.UserSubscriptions.Events;

namespace Pento.Application.EventHandlers.Users;

internal sealed class UserSubscriptionExpiredEventHandler(
    INotificationService notificationService,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository
    ) : DomainEventHandler<UserSubscriptionExpiredDomainEvent>
{
    public override async Task Handle(
        UserSubscriptionExpiredDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(domainEvent.UserSubscriptionId, cancellationToken);
        if (userSubscription == null)
        {
            throw new PentoException(nameof(UserSubscriptionExpiredEventHandler), SubscriptionErrors.UserSubscriptionNotFound);
        }
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            throw new PentoException(nameof(UserSubscriptionExpiredEventHandler), SubscriptionErrors.SubscriptionNotFound);
        }
        string title = "Subscription Expired";
        string body = $"Your {subscription.Name} subscription has expired.";
        var payload = new Dictionary<string, string>
        {
            { "subscriptionId", subscription.Id.ToString() },
            { "SubscriptionName", subscription.Name  },
            {  "userSubscriptionId", userSubscription.Id.ToString()   }
        };
        Result notificationResult = await notificationService.SendToUserAsync(
            userSubscription.UserId,
            title,
            body,
            NotificationType.Subscription,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(UserSubscriptionExpiredEventHandler), notificationResult.Error);
        }
    }
}
