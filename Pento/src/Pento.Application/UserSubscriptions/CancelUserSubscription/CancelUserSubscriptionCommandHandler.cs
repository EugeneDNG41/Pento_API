using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.UserSubscriptions.CancelUserSubscription;

internal sealed class CancelUserSubscriptionCommandHandler(
    ISubscriptionService subscriptionService,
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CancelUserSubscriptionCommand>
{
    public async Task<Result> Handle(CancelUserSubscriptionCommand command, CancellationToken cancellationToken)
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
        if (userSubscription.Status == SubscriptionStatus.Expired)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionExpired);
        }
        if (userSubscription.Status != SubscriptionStatus.Cancelled)
        {
            userSubscription.Cancel(dateTimeProvider.Today, command.Reason);
            await userSubscriptionRepository.UpdateAsync(userSubscription, cancellationToken);
            Result deactivationResult = await subscriptionService.DeactivateAsync(userSubscription, cancellationToken);
            if (deactivationResult.IsFailure)
            {
                return Result.Failure(deactivationResult.Error);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
}

