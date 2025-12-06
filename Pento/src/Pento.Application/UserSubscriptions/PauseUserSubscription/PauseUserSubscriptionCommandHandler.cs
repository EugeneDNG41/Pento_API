using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.UserSubscriptions.PauseUserSubscription;

internal sealed class PauseUserSubscriptionCommandHandler(
    INotificationService notificationService,
    ISubscriptionService subscriptionService,
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<PauseUserSubscriptionCommand>
{
    public async Task<Result> Handle(PauseUserSubscriptionCommand request, CancellationToken cancellationToken)
    {
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
        else if (userSubscription.Status == SubscriptionStatus.Paused)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionPaused);
        }
        else if (!userSubscription.EndDate.HasValue)
        {
            return Result.Failure(SubscriptionErrors.CannotPauseLifetimeSubscription);
        }
        else
        {
            userSubscription.Pause(dateTimeProvider.Today);
            userSubscriptionRepository.Update(userSubscription);
            Result deactivationResult = await subscriptionService.DeactivateAsync(userSubscription, cancellationToken);
            if (deactivationResult.IsFailure)
            {
                return Result.Failure(deactivationResult.Error);
            }
            string title = "Subscription Paused";
            string body = $"Your {subscription.Name} subscription has been put on hold.";
            var payload = new Dictionary<string, string>
            {
                { "userSubscriptionId", userSubscription.Id.ToString() },
                { "subscriptionId", subscription.Id.ToString() },
                { "subscriptionName", subscription.Name  }
            };
            Result notificationResult = await notificationService
                .SendToUserAsync(userSubscription.UserId, title, body, NotificationType.Subscription, payload, cancellationToken);
            if (notificationResult.IsFailure)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken); //since service won't call save changes
            }
            return Result.Success();
        }
    }
}

