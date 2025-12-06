using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Payments;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;
using Pento.Domain.Users;
using Pento.Domain.UserSubscriptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Domain.Notifications;

namespace Pento.Application.EventHandlers;

internal sealed class PaymentCompletedEventHandler(
    IDateTimeProvider dateTimeProvider,
    INotificationService notificationService,
    ISubscriptionService subscriptionService,
    IGenericRepository<Subscription> subscriptionRepository,
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
        SubscriptionPlan? plan = await subscriptionPlanRepository.GetByIdAsync(payment.SubscriptionPlanId, cancellationToken);
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
            us => us.SubscriptionId == plan.SubscriptionId && us.UserId == payment.UserId, 
            cancellationToken)).SingleOrDefault();
        var payload = new Dictionary<string, string>
        {
            { "subscriptionId", plan.SubscriptionId.ToString() },
            { "subscriptionPlanId", plan.Id.ToString() },
            { "paymentId", payment.Id.ToString() }
        };
        string title = "Subscription ";
        string body = $"Your {subscription.Name} subscription has been .";
        if (userSubscription == null)
        {
            var newUserSubscription = UserSubscription.Create(
                payment.UserId,
                plan.SubscriptionId,
                dateTimeProvider.Today,
                plan.DurationInDays is null ? null : dateTimeProvider.Today.AddDays(plan.DurationInDays.Value));
            userSubscriptionRepository.Add(newUserSubscription);
            userSubscription = newUserSubscription;
            title += "Activated";
            body += "activated";
        } 
        else
        {
            userSubscription.Renew(plan.DurationInDays is null ? null : dateTimeProvider.Today.AddDays(plan.DurationInDays.Value));
            userSubscriptionRepository.Update(userSubscription);
            title += "Renewed";
            body += "renewed";
        }
        Result activationResult = await subscriptionService.ActivateAsync(
            userSubscription,
            cancellationToken);
        if (activationResult.IsFailure)
        {
            throw new PentoException(
                nameof(PaymentCompletedEventHandler),
                activationResult.Error);
        }
        Result notificationResult = await notificationService.SendToUserAsync(
            payment.UserId,
            title,
            body,
            NotificationType.Subscription,
            payload,
            cancellationToken);
        if (notificationResult.IsFailure)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            throw new PentoException(nameof(PaymentCompletedEventHandler), notificationResult.Error);
        }
    }
}
