using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;
using Pento.Application.Abstractions.External.Firebase;
using System.Globalization;
using Pento.Domain.Notifications;

namespace Pento.Application.UserSubscriptions.AdjustUserSubscription;

internal sealed class AdjustUserSubscriptionCommandHandler(
    INotificationService notificationService,
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AdjustUserSubscriptionCommand>
{
    public async Task<Result> Handle(AdjustUserSubscriptionCommand command, CancellationToken cancellationToken)
    {
        UserSubscription? userSubscription = await userSubscriptionRepository.GetByIdAsync(command.UserSubscriptionId, cancellationToken);
        if (userSubscription == null)
        {
            return Result.Failure(SubscriptionErrors.UserSubscriptionNotFound);
        }
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(userSubscription.SubscriptionId, cancellationToken);
        if (subscription == null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionNotFound);
        }
        if (userSubscription.Status == SubscriptionStatus.Cancelled)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionCancelled);
        }
        else if (userSubscription.Status == SubscriptionStatus.Expired)
        {
            userSubscription.Renew(dateTimeProvider.Today.AddDays(command.DurationInDays));
        }
        else if (userSubscription.Status == SubscriptionStatus.Active && !userSubscription.EndDate.HasValue)
        {
            return Result.Failure(SubscriptionErrors.CannotExtendLifetimeSubscription);
        }
        else if (userSubscription.EndDate.HasValue && userSubscription.EndDate.Value.AddDays(command.DurationInDays) <= dateTimeProvider.Today 
            || userSubscription.RemainingDaysAfterPause.HasValue && userSubscription.RemainingDaysAfterPause.Value + command.DurationInDays <= 0)
        {
            return Result.Failure(SubscriptionErrors.CannotReduceBelowRemainingDay);
        }
        else
        {
            userSubscription.Adjust(command.DurationInDays);
        }
        userSubscriptionRepository.Update(userSubscription);

        string title = "Subscription Adjusted";
        string adjustmentType = command.DurationInDays > 0 ? "extended" : "reduced";
        string body = $"Your subscription '{subscription.Name}' has been {adjustmentType} by {Math.Abs(command.DurationInDays)} day(s).";
        var payload = new Dictionary<string, string>
        {
            { "UserSubscriptionId", userSubscription.Id.ToString() },
            { "SubscriptionId", subscription.Id.ToString() },
            { "SubscriptionName", subscription.Name  },
            { "AdjustmentInDays", command.DurationInDays.ToString(CultureInfo.InvariantCulture) },
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

