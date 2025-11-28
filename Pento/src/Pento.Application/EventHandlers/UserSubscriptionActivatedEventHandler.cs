using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.EventHandlers;

internal sealed class UserSubscriptionActivatedEventHandler(
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IUnitOfWork unitOfWork
    ) : DomainEventHandler<UserSubscriptionActivatedDomainEvent>
{
    public override async Task Handle(UserSubscriptionActivatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(domainEvent.UserSubscriptionId, cancellationToken);
        if (userSubscription == null)
        {
            throw new PentoException(nameof(UserSubscriptionActivatedEventHandler), SubscriptionErrors.UserSubscriptionNotFound);
        }
        var subscriptionFeatures = (await subscriptionFeatureRepository.FindAsync(uf => uf.SubscriptionId == userSubscription.SubscriptionId, cancellationToken)).ToList();
        foreach (SubscriptionFeature? subscriptionFeature in subscriptionFeatures)
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
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
