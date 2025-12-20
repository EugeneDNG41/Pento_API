using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Features;
using Pento.Domain.Notifications;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserEntitlements.Events;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.EventHandlers.Users.Entitlements;

internal sealed class UserEntitlementUpdatedEventHandler(
    INotificationService notificationService,
    IGenericRepository<UserEntitlement> entitlementRepository,
    IGenericRepository<Feature> featureRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IGenericRepository<Subscription> subscriptionRepository
    ) : DomainEventHandler<EntitlementUpdatedDomainEvent>
{
    public async override Task Handle(EntitlementUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        UserEntitlement? entitlement = await entitlementRepository.GetByIdAsync(domainEvent.UserEntitlementId, cancellationToken);
        if (entitlement == null)
        {
            throw new PentoException(nameof(UserEntitlementUpdatedEventHandler), UserEntitlementErrors.NotFound);

        }
        Feature? feature = (await featureRepository.FindAsync(f => f.Code == entitlement.FeatureCode, cancellationToken)).SingleOrDefault();
        if (feature == null)
        {
            throw new PentoException(nameof(UserEntitlementUpdatedEventHandler), FeatureErrors.NotFound);
        }
        string title = $"Entitlement Updated";
        string body = $"Your entitlement for {feature.Name} has been updated due to a change in the ";
        var payload = new Dictionary<string, string>
        {
            { "featureCode", feature.Code },
            { "featureName", feature.Name  },
            { "userEntitlementId", entitlement.Id.ToString() }
        };
        if (entitlement.UserSubscriptionId != null)
        {
            UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(entitlement.UserSubscriptionId.Value, cancellationToken);
            if (userSubscription == null)
            {
                throw new PentoException(nameof(UserEntitlementUpdatedEventHandler), SubscriptionErrors.UserSubscriptionNotFound);
            }
            Subscription? subscription = await subscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId, cancellationToken);
            if (subscription == null)
            {
                throw new PentoException(nameof(UserEntitlementUpdatedEventHandler), SubscriptionErrors.SubscriptionNotFound);
            }
            body += $"{subscription.Name} subscription's entitlement(s).";
            payload.Add("subscriptionId", subscription.Id.ToString());
        }
        else
        {
            body += "default policy.";
        }
        Result notificationResult = await notificationService.SendToUserAsync(
            entitlement.UserId,
            title,
            body,
            NotificationType.Entitlement,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(UserEntitlementUpdatedEventHandler), notificationResult.Error);
        }
    }
}
