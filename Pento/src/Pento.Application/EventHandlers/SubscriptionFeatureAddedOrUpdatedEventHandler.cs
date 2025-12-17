using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.EventHandlers;

internal sealed class SubscriptionFeatureAddedOrUpdatedEventHandler( //business logic to update user entitlements when subscription feature is added or updated
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<SubscriptionFeatureAddedOrUpdatedDomainEvent>
{
    public override async Task Handle(
        SubscriptionFeatureAddedOrUpdatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        SubscriptionFeature? subscriptionFeature = await subscriptionFeatureRepository.GetByIdAsync(
            domainEvent.SubscriptionFeatureId,
            cancellationToken);
        if (subscriptionFeature == null)
        {
            throw new PentoException(
                nameof(SubscriptionFeatureAddedOrUpdatedEventHandler),
                SubscriptionErrors.SubscriptionFeatureNotFound);
        }
        IEnumerable<UserSubscription> activeUserSubscriptions = await userSubscriptionRepository.FindAsync(
            us => us.SubscriptionId == subscriptionFeature.SubscriptionId
            && us.Status == SubscriptionStatus.Active,
            cancellationToken);
        if (activeUserSubscriptions.Any())
        {
            foreach (UserSubscription userSubscription in activeUserSubscriptions)
            {
                UserEntitlement? userEntitlement = (await userEntitlementRepository.FindAsync(
                    ue => ue.UserSubscriptionId == userSubscription.Id
                    && ue.FeatureCode == subscriptionFeature.FeatureCode,
                    cancellationToken)).SingleOrDefault();
                if (userEntitlement == null)
                {
                    var newUserEntitlement = UserEntitlement.Create(
                        userSubscription.UserId,
                        userSubscription.Id,
                        subscriptionFeature.FeatureCode,
                        subscriptionFeature.Quota,
                        subscriptionFeature.ResetPeriod);
                    userEntitlementRepository.Add(newUserEntitlement);
                }
                else
                {
                    userEntitlement.UpdateEntitlement(
                        subscriptionFeature.Quota,
                        subscriptionFeature.ResetPeriod);
                    await userEntitlementRepository.UpdateAsync(userEntitlement, cancellationToken);
                }
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
