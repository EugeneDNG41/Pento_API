using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserEntitlements;
using Pento.Domain.Users;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.EventHandlers;

internal sealed class PaymentCompletedEventHandler(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IGenericRepository<User> userRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IUnitOfWork unitOfWork
    ) : DomainEventHandler<PaymentCompletedDomainEvent>
{
    public override async Task Handle(
        PaymentCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        User? user = await userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
        if (user == null)
        {
            throw new PentoException(nameof(PaymentCompletedEventHandler), UserErrors.NotFound);
        }
        SubscriptionPlan? plan = await subscriptionPlanRepository.GetByIdAsync(domainEvent.SubscriptionPlanId, cancellationToken);
        if (plan == null)
        {
            throw new PentoException(nameof(PaymentCompletedEventHandler), SubscriptionErrors.SubscriptionPlanNotFound);
        }
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(plan.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            throw new PentoException(nameof(PaymentCompletedEventHandler), SubscriptionErrors.SubscriptionNotFound);
        }
        UserSubscription? userSubscription = (await userSubscriptionRepository.FindAsync(
            us => us.SubscriptionId == subscription.Id && us.UserId == domainEvent.UserId, 
            cancellationToken)).SingleOrDefault();
        if (userSubscription == null)
        {

            var newUserSubscription = UserSubscription.Create(
                domainEvent.UserId,
                subscription.Id,
                SubscriptionStatus.Active,
                dateTimeProvider.Today,
                plan.DurationInDays is null ? null : dateTimeProvider.Today.AddDays(plan.DurationInDays.Value));
            userSubscriptionRepository.Add(newUserSubscription);
            userSubscription = newUserSubscription;
        } 
        else
        {
            userSubscription.Renew(
                plan.DurationInDays is null ? null : dateTimeProvider.Today.AddDays(plan.DurationInDays.Value));
            userSubscriptionRepository.Update(userSubscription);
        }
        var features = (await subscriptionFeatureRepository.FindAsync(uf => uf.SubscriptionId == subscription.Id, cancellationToken)).ToList();
        foreach (SubscriptionFeature? subscriptionFeature in features)
        {
            UserEntitlement? existingEntitlement = (await userEntitlementRepository.FindAsync(
                    ue => ue.UserId == domainEvent.UserId && ue.FeatureCode == subscriptionFeature.FeatureCode && ue.UserSubscriptionId == userSubscription.Id,
                    cancellationToken)).SingleOrDefault();
            if (existingEntitlement == null)
            {
                var entitlement = UserEntitlement.Create(domainEvent.UserId, userSubscription.Id, subscriptionFeature.FeatureCode, subscriptionFeature.Quota, subscriptionFeature.ResetPeriod);
                userEntitlementRepository.Add(entitlement);
            }
            else if (existingEntitlement.Quota != subscriptionFeature.Quota || existingEntitlement.ResetPeriod != subscriptionFeature.ResetPeriod) 
            {
                existingEntitlement.UpdateEntitlement(subscriptionFeature.Quota, subscriptionFeature.ResetPeriod);
                userEntitlementRepository.Update(existingEntitlement);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

