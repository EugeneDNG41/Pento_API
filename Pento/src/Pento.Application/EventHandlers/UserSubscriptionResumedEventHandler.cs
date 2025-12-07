using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;
using Pento.Domain.UserSubscriptions.Events;

namespace Pento.Application.EventHandlers;

internal sealed class UserSubscriptionResumedEventHandler(
    INotificationService notificationService,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository
    ) : DomainEventHandler<UserSubscriptionResumedDomainEvent>
{
    public override async Task Handle(
        UserSubscriptionResumedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(domainEvent.UserSubscriptionId, cancellationToken);
        if (userSubscription == null)
        {
            throw new PentoException(nameof(UserSubscriptionResumedEventHandler), SubscriptionErrors.UserSubscriptionNotFound);
        }
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            throw new PentoException(nameof(UserSubscriptionResumedEventHandler), SubscriptionErrors.SubscriptionNotFound);
        }
        string title = "Subscription Resumed";
        string body = $"Your {subscription.Name} subscription has been resumed.";
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
            throw new PentoException(nameof(UserSubscriptionResumedEventHandler), notificationResult.Error);
        }
    }
}
