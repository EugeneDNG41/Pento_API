using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;
using Quartz;
namespace Pento.Infrastructure.External.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessSubscriptionTrackingJob(
    INotificationService notificationSender,
    IDateTimeProvider dateTimeProvider,
    ISubscriptionService subscriptionService,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork
    ) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        DateOnly today = dateTimeProvider.Today;
        var activeSubscriptions = (await userSubscriptionRepository
                    .FindAsync(s => s.Status == SubscriptionStatus.Active && s.EndDate != null && s.EndDate.Value.DayNumber - today.DayNumber <= 1, context.CancellationToken)).ToList();
        foreach (UserSubscription userSubscription in activeSubscriptions)
        {
            Subscription? subscription = await subscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId, context.CancellationToken);
            if (subscription == null)
            {
                throw new PentoException(nameof(ProcessSubscriptionTrackingJob), SubscriptionErrors.SubscriptionNotFound);
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
                Result notificationResult = await notificationSender.SendToUserAsync(userSubscription.UserId, title, body, NotificationType.Subscription, payload, context.CancellationToken);
                if (notificationResult.IsFailure)
                {
                    throw new PentoException(nameof(ProcessSubscriptionTrackingJob), notificationResult.Error);
                }
            }
            else
            {
                userSubscription.Expire();
                Result deactivationResult = await subscriptionService.DeactivateAsync(userSubscription, context.CancellationToken);
                if (deactivationResult.IsFailure)
                {
                    throw new PentoException(nameof(ProcessSubscriptionTrackingJob), deactivationResult.Error);
                }
            }
        }
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

