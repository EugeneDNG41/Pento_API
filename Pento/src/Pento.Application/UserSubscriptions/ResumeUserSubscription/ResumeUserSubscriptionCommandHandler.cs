using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.UserSubscriptions.ResumeUserSubscription;

internal sealed class ResumeUserSubscriptionCommandHandler(
    INotificationService notificationService,
    ISubscriptionService subscriptionService,
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ResumeUserSubscriptionCommand>
{
    public async Task<Result> Handle(ResumeUserSubscriptionCommand request, CancellationToken cancellationToken)
    {
        DateOnly today = dateTimeProvider.Today;
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(request.UserSubscriptionId, cancellationToken);
        if (userSubscription == null)
        {
            return Result.Failure(SubscriptionErrors.UserSubscriptionNotFound);
        }
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionNotFound);
        }
        if (userSubscription.Status == SubscriptionStatus.Expired)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionExpired);
        }
        else if (userSubscription.Status == SubscriptionStatus.Cancelled)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionCancelled);
        }
        else if (userSubscription.Status == SubscriptionStatus.Active)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionActive);
        }
        else if (!userSubscription.EndDate.HasValue)
        {
            return Result.Failure(SubscriptionErrors.CannotPauseLifetimeSubscription);
        }
        else
        {
            int minPauseDays = 1; // business rule
            if (userSubscription.PausedDate.HasValue && today.DayNumber - userSubscription.PausedDate.Value.DayNumber < minPauseDays)
            {
                return Result.Failure(SubscriptionErrors.MinimumPauseDay(minPauseDays));
            }
            userSubscription.Resume(dateTimeProvider.Today);
            userSubscriptionRepository.Update(userSubscription);

            Result reactivationResult = await subscriptionService.ActivateAsync(userSubscription, cancellationToken);
            if (reactivationResult.IsFailure)
            {
                return Result.Failure(reactivationResult.Error);
            }

            string title = "Subscription Resumed";
            string body = $"Your {subscription.Name} subscription has been resumed.";
            var payload = new Dictionary<string, string>
            {
                { "UserSubscriptionId", userSubscription.Id.ToString() },
                { "SubscriptionId", subscription.Id.ToString() },
                { "SubscriptionName", subscription.Name  },
            };
            Result notificationResult = await notificationService.SendToUserAsync(
                userSubscription.UserId,
                title,
                body,
                NotificationType.Subscription,
                payload,
                cancellationToken);
            if (notificationResult.IsFailure)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            return Result.Success();

        }
    }
}

