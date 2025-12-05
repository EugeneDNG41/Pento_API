using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.DomainServices;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserSubscriptions;
using Quartz;
namespace Pento.Infrastructure.ThirdPartyServices.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessLimitResetJob(
    IDateTimeProvider dateTimeProvider,
    ISubscriptionService subscriptionService,
    INotificationService notificationSender,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork
    ) : IJob

{
    public async Task Execute(IJobExecutionContext context)
    {
        DateOnly today = dateTimeProvider.Today;
        await ExecuteSubscriptionTracking(today, context.CancellationToken);
        await ExecuteEntitlementReset(today, context.CancellationToken);
    }


    private async Task ExecuteEntitlementReset(DateOnly today, CancellationToken cancellationToken)
    {
        var resettableEntitlements = (await userEntitlementRepository.FindAsync(e => e.ResetPeriod != null, cancellationToken)).ToList();
        foreach (UserEntitlement entitlement in resettableEntitlements)
        {
            UserSubscription? subscriptionInstance = (await userSubscriptionRepository
                .FindAsync(s => s.UserId == entitlement.UserId && s.Status == SubscriptionStatus.Active, cancellationToken))
                .LastOrDefault();
            if (entitlement.ResetPeriod == TimeUnit.Day)
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Week &&
                (subscriptionInstance == null && today.DayOfWeek == DayOfWeek.Monday ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 7 == 0))
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Month &&
                (subscriptionInstance == null && today.Day == 1 ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 30 == 0))
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Year &&
                (subscriptionInstance == null && today.Day == 1 && today.Month == 1  ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 365 == 0))
            {
                entitlement.ResetUsage();
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
    private async Task ExecuteSubscriptionTracking(DateOnly today, CancellationToken cancellationToken)
    {
        var activeSubscriptions = (await userSubscriptionRepository
            .FindAsync(s => s.Status == SubscriptionStatus.Active && s.EndDate != null && s.EndDate.Value.DayNumber - today.DayNumber <= 1, cancellationToken)).ToList();
        foreach (UserSubscription userSubscription in activeSubscriptions)
        {
            Subscription? subscription = await subscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId, cancellationToken);
            if (subscription == null)
            {
                throw new PentoException(nameof(ExecuteSubscriptionTracking), SubscriptionErrors.SubscriptionNotFound);
            }
            var payload = new Dictionary<string, string>
                {
                    { "subscriptionId", userSubscription.SubscriptionId.ToString() },
                    { "subscriptionName", subscription.Name },
                    { "userSubscriptionId", userSubscription.Id.ToString() }
                };
            if (userSubscription.EndDate!.Value.DayNumber - today.DayNumber == 1)
            {
                string title = "Subscription Expiry Reminder";
                string body = $"Your {subscription.Name} Subscription for the Pento app is set to expire tomorrow. Please renew to continue enjoying our services without interruption.";
                Result notificationResult = await notificationSender.SendToUserAsync(userSubscription.UserId, title, body, NotificationType.Subscription, payload, cancellationToken);
                if (notificationResult.IsFailure)
                {
                    throw new PentoException(nameof(ExecuteSubscriptionTracking), notificationResult.Error);
                }
            }
            else 
            {
                userSubscription.Expire();
                Result deactivationResult = await subscriptionService.DeactivateAsync(userSubscription, cancellationToken);
                if (deactivationResult.IsFailure)
                {
                    throw new PentoException(nameof(ExecuteSubscriptionTracking), deactivationResult.Error);
                }
                string title = "Subscription Expired";
                string body = $"Your {subscription.Name} Subscription for the Pento app has expired. Please renew to continue enjoying our services.";

                Result notificationResult = await notificationSender.SendToUserAsync(userSubscription.UserId, title, body, NotificationType.Subscription, payload, cancellationToken);
                if (notificationResult.IsFailure)
                {
                    throw new PentoException(nameof(ExecuteSubscriptionTracking), notificationResult.Error);
                }
                
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

