using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Users.PauseUserSubscription;

internal sealed class PauseUserSubscriptionCommandHandler(
    IDateTimeProvider dateTimeProvider,
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
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

