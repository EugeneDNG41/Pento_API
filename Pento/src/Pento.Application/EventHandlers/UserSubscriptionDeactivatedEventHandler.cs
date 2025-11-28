using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.EventHandlers;

internal sealed class UserSubscriptionDeactivatedEventHandler(
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IGenericRepository<Feature> featureRepository,
    IUnitOfWork unitOfWork
    ) : DomainEventHandler<UserSubscriptionDeactivatedDomainEvent>
{
    public override async Task Handle(UserSubscriptionDeactivatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(domainEvent.UserSubscriptionId, cancellationToken);
        if (userSubscription == null)
        {
            throw new PentoException(nameof(UserSubscriptionDeactivatedDomainEvent), SubscriptionErrors.UserSubscriptionNotFound);
        }
        var subscriptionFeatures = (await subscriptionFeatureRepository.FindAsync(uf => uf.SubscriptionId == userSubscription.SubscriptionId, cancellationToken)).ToList();
        foreach (SubscriptionFeature? subscriptionFeature in subscriptionFeatures)
        {
            Feature? feature = (await featureRepository.FindAsync(f => f.Code == subscriptionFeature.FeatureCode, cancellationToken)).SingleOrDefault();
            if (feature == null)
            {
                throw new PentoException(nameof(UserSubscriptionDeactivatedDomainEvent), FeatureErrors.NotFound);
            }
            UserEntitlement? thisSubscriptionEntitlement = (await userEntitlementRepository.FindAsync(
                    ue => ue.FeatureCode == subscriptionFeature.FeatureCode && ue.UserSubscriptionId == userSubscription.Id,
                    cancellationToken)).SingleOrDefault();
            bool otherSubscriptionEntitlementsExist = await userEntitlementRepository.AnyAsync(
                    ue => ue.UserId == userSubscription.UserId &&
                    ue.FeatureCode == subscriptionFeature.FeatureCode &&
                    ue.UserSubscriptionId != null &&
                    ue.UserSubscriptionId != userSubscription.Id,
                    cancellationToken);
            if (thisSubscriptionEntitlement == null && feature.DefaultQuota != null && !otherSubscriptionEntitlementsExist)
            {
                var entitlement = UserEntitlement.Create(userSubscription.UserId, null, subscriptionFeature.FeatureCode, feature.DefaultQuota, feature.DefaultResetPeriod);
                userEntitlementRepository.Add(entitlement);
            }
            else if (thisSubscriptionEntitlement != null && (feature.DefaultQuota == null || otherSubscriptionEntitlementsExist))
            {
                userEntitlementRepository.Remove(thisSubscriptionEntitlement);
            }
            else if (thisSubscriptionEntitlement != null && !otherSubscriptionEntitlementsExist)
            {
                thisSubscriptionEntitlement.UpdateEntitlement(feature.DefaultQuota, feature.DefaultResetPeriod);
                userEntitlementRepository.Update(thisSubscriptionEntitlement);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
