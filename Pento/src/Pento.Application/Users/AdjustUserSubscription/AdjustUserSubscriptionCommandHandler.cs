using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Users.AdjustUserSubscription;

internal sealed class AdjustUserSubscriptionCommandHandler(
    IDateTimeProvider dateTimeProvider,
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
            userSubscription.Extend(command.DurationInDays);
        }
        userSubscriptionRepository.Update(userSubscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

