using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Features;
using Pento.Domain.Notifications;
using Pento.Domain.Shared;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserEntitlements.Events;

namespace Pento.Application.EventHandlers.Users.Entitlements;

internal sealed class UserEntitlementResetEventHandler(
    INotificationService notificationService,
    IGenericRepository<UserEntitlement> entitlementRepository,
    IGenericRepository<Feature> featureRepository) : DomainEventHandler<EntitlementResetDomainEvent>
{
    public async override Task Handle(EntitlementResetDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        UserEntitlement? entitlement = await entitlementRepository.GetByIdAsync(domainEvent.UserEntitlementId, cancellationToken);
        if (entitlement == null)
        {
            throw new PentoException(nameof(UserEntitlementResetEventHandler), UserEntitlementErrors.NotFound);

        }
        Feature? feature = (await featureRepository.FindAsync(f => f.Code == entitlement.FeatureCode, cancellationToken)).SingleOrDefault();
        if (feature == null)
        {
            throw new PentoException(nameof(UserEntitlementResetEventHandler), FeatureErrors.NotFound);
        }
        string resetPeriod = feature.DefaultResetPeriod switch
        {
            TimeUnit.Day => "Daily",
            TimeUnit.Week => "Weekly",
            TimeUnit.Month => "Monthly",
            TimeUnit.Year => "Yearly",
            _ => throw new PentoException(nameof(UserEntitlementResetEventHandler), UserEntitlementErrors.InvalidReset)
        };
        string title = $"{resetPeriod} Entitlement Reset";
        string body = $"Your entitlement for {feature.Name} has been reset. You can now use it again.";
        var payload = new Dictionary<string, string>
        {
            { "featureCode", feature.Code },
            { "featureName", feature.Name  },
            { "userEntitlementId", entitlement.Id.ToString() }
        };
        Result notificationResult = await notificationService.SendToUserAsync(
            entitlement.UserId,
            title,
            body,
            NotificationType.Entitlement,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            throw new PentoException(nameof(UserEntitlementResetEventHandler), notificationResult.Error);
        }
    }
}
