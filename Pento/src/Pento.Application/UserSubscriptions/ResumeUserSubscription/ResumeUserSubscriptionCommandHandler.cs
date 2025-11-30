using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.UserSubscriptions.ResumeUserSubscription;

internal sealed class ResumeUserSubscriptionCommandHandler(
    IDateTimeProvider dateTimeProvider,
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
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}

