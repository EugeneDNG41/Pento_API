using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Features;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserSubscriptions;
using Pento.Application.Abstractions.External.Firebase;

namespace Pento.Infrastructure.Services;

internal sealed class SubscriptionService(
    IGenericRepository<Feature> featureRepository,
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository) : ISubscriptionService
{
    public async Task<Result> ActivateAsync(UserSubscription userSubscription, CancellationToken cancellationToken)
    {
        var subscriptionFeatures = (await subscriptionFeatureRepository.FindAsync(uf => uf.SubscriptionId == userSubscription.SubscriptionId, cancellationToken)).ToList();
        foreach (SubscriptionFeature subscriptionFeature in subscriptionFeatures)
        {
            UserEntitlement? existingEntitlement = (await userEntitlementRepository.FindAsync(
                    ue => ue.UserId == userSubscription.UserId &&
                    ue.FeatureCode == subscriptionFeature.FeatureCode &&
                    ue.UserSubscriptionId == null ||
                    ue.UserSubscriptionId == userSubscription.Id,
                    cancellationToken)).SingleOrDefault();
            if (existingEntitlement == null)
            {
                var entitlement = UserEntitlement.Create(userSubscription.UserId, userSubscription.Id, subscriptionFeature.FeatureCode, subscriptionFeature.Quota, subscriptionFeature.ResetPeriod);
                userEntitlementRepository.Add(entitlement);
            }
            else if (existingEntitlement.Quota != subscriptionFeature.Quota || existingEntitlement.ResetPeriod != subscriptionFeature.ResetPeriod)
            {
                existingEntitlement.UpdateEntitlement(subscriptionFeature.Quota, subscriptionFeature.ResetPeriod, userSubscription.Id);
                userEntitlementRepository.Update(existingEntitlement);
            }
        }
        return Result.Success();
    }
    public async Task<Result> DeactivateAsync(UserSubscription userSubscription, CancellationToken cancellationToken)
    {
        var subscriptionFeatures = (await subscriptionFeatureRepository.FindAsync(uf => uf.SubscriptionId == userSubscription.SubscriptionId, cancellationToken)).ToList();
        foreach (SubscriptionFeature? subscriptionFeature in subscriptionFeatures)
        {
            Feature? feature = (await featureRepository.FindAsync(f => f.Code == subscriptionFeature.FeatureCode, cancellationToken)).SingleOrDefault();
            if (feature == null)
            {
                return Result.Failure(FeatureErrors.NotFound);
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
        return Result.Success();
    }
}
