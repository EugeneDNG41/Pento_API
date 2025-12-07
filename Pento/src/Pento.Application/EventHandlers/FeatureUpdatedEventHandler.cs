using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Features;
using Pento.Domain.UserEntitlements;

namespace Pento.Application.EventHandlers;

internal sealed class FeatureUpdatedEventHandler( //business logic to update user entitlements when feature is updated
    IGenericRepository<Feature> featureRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<FeatureUpdatedDomainEvent>
{
    public override async Task Handle(
        FeatureUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Feature? feature = (await featureRepository.FindAsync(f => f.Code ==
            domainEvent.FeatureCode,
            cancellationToken)).SingleOrDefault();
        if (feature == null)
        {
            throw new PentoException(
                nameof(FeatureUpdatedEventHandler),
                FeatureErrors.NotFound);
        }
        IEnumerable<UserEntitlement> userEntitlements = await userEntitlementRepository.FindAsync(
            ue => ue.FeatureCode == domainEvent.FeatureCode
            && ue.UserSubscriptionId == null
            && (ue.Quota != feature.DefaultQuota
                || ue.ResetPeriod != feature.DefaultResetPeriod),
            cancellationToken);
        if (userEntitlements.Any())
        {
            foreach (UserEntitlement userEntitlement in userEntitlements)
            {
                if (feature.DefaultQuota == null && feature.DefaultResetPeriod == null)
                {
                    userEntitlementRepository.Remove(userEntitlement);
                }
                else
                {
                    userEntitlement.UpdateEntitlement(
                        feature.DefaultQuota,
                        feature.DefaultResetPeriod);
                }
            }
            userEntitlementRepository.UpdateRange(userEntitlements);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
