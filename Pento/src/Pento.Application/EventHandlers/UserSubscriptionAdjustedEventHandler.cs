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

internal sealed class UserSubscriptionAdjustedEventHandler(
    INotificationService notificationService,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository
    ) : DomainEventHandler<UserSubscriptionAdjustedDomainEvent>
{
    public override async Task Handle(
        UserSubscriptionAdjustedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(domainEvent.UserSubscriptionId, cancellationToken);
        if (userSubscription == null)
        {
            throw new PentoException(nameof(UserSubscriptionAdjustedEventHandler), SubscriptionErrors.UserSubscriptionNotFound);
        }
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            throw new PentoException(nameof(UserSubscriptionAdjustedEventHandler), SubscriptionErrors.SubscriptionNotFound);
        }
        string title = "Subscription Adjusted";
        string adjustmentType = domainEvent.DurationInDays > 0 ? "extended" : "reduced";
        string body = $"Your subscription '{subscription.Name}' has been {adjustmentType} by {Math.Abs(domainEvent.DurationInDays)} day(s).";
        var payload = new Dictionary<string, string>
        {
            { "UserSubscriptionId", userSubscription.Id.ToString() },
            { "SubscriptionId", subscription.Id.ToString() },
            { "SubscriptionName", subscription.Name  },
            { "AdjustmentInDays", domainEvent.DurationInDays.ToString(System.Globalization.CultureInfo.InvariantCulture) },
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
            throw new PentoException(nameof(UserSubscriptionAdjustedEventHandler), notificationResult.Error);
        }
    }
}
