using System.Globalization;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.UserSubscriptions.ResumeSubscription;

internal sealed class ResumeSubscriptionCommandHandler(
    INotificationService notificationService,
    ISubscriptionService subscriptionService,
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ResumeSubscriptionCommand>
{
    public async Task<Result> Handle(ResumeSubscriptionCommand request, CancellationToken cancellationToken)
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
        if (userSubscription.UserId != userContext.UserId)
        {
            return Result.Failure(SubscriptionErrors.ForbiddenAccess);
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

