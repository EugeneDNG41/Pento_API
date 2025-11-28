using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;
using Pento.Domain.Users;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.EventHandlers;

internal sealed class PaymentCompletedEventHandler(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IGenericRepository<Payment> paymentRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork
    ) : DomainEventHandler<PaymentCompletedDomainEvent>
{
    public override async Task Handle(
        PaymentCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Payment? payment = await paymentRepository.GetByIdAsync(domainEvent.PaymentId, cancellationToken);
        if (payment == null)
        {
            throw new PentoException(nameof(PaymentCompletedEventHandler), UserErrors.NotFound);
        }
        SubscriptionPlan? plan = await subscriptionPlanRepository.GetByIdAsync(domainEvent.SubscriptionPlanId, cancellationToken);
        if (plan == null)
        {
            throw new PentoException(nameof(PaymentCompletedEventHandler), SubscriptionErrors.SubscriptionPlanNotFound);
        }
        UserSubscription? userSubscription = (await userSubscriptionRepository.FindAsync(
            us => us.SubscriptionId == plan.SubscriptionId && us.UserId == payment.UserId, 
            cancellationToken)).SingleOrDefault();
        if (userSubscription == null)
        {
            var newUserSubscription = UserSubscription.Create(
                payment.UserId,
                plan.SubscriptionId,
                dateTimeProvider.Today,
                plan.DurationInDays is null ? null : dateTimeProvider.Today.AddDays(plan.DurationInDays.Value));
            userSubscriptionRepository.Add(newUserSubscription);
        } 
        else
        {
            userSubscription.Renew(plan.DurationInDays is null ? null : dateTimeProvider.Today.AddDays(plan.DurationInDays.Value));
            userSubscriptionRepository.Update(userSubscription);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
