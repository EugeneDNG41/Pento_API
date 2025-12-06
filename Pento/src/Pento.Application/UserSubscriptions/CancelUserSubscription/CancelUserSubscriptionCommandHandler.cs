using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.UserSubscriptions.CancelUserSubscription;

internal sealed class CancelUserSubscriptionCommandHandler(
    ISubscriptionService subscriptionService,
    IDateTimeProvider dateTimeProvider,
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
        if (userSubscription.Status == SubscriptionStatus.Expired)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionExpired);
        }
        if (userSubscription.Status != SubscriptionStatus.Cancelled)
        {
            userSubscription.Cancel(dateTimeProvider.Today, command.Reason);
            userSubscriptionRepository.Update(userSubscription);
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

