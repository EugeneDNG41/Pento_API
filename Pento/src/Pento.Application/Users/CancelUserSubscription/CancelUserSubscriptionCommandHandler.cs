using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Users.CancelUserSubscription;

internal sealed class CancelUserSubscriptionCommandHandler(
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
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
}

