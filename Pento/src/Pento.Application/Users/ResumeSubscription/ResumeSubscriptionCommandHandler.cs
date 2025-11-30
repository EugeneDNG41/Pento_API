using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Users.ResumeSubscription;

internal sealed class ResumeSubscriptionCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
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
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}

