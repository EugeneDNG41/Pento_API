using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Subscriptions.Delete;

internal sealed class DeleteSubscriptionCommandHandler(
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteSubscriptionCommand>
{
    public async Task<Result> Handle(DeleteSubscriptionCommand command, CancellationToken cancellationToken)
    {
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (subscription is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionNotFound);
        }
        bool isSubscriptionInUse = await userSubscriptionRepository.AnyAsync(
            us => us.SubscriptionId == command.Id,
            cancellationToken);
        if (isSubscriptionInUse)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionInUse);
        }
        subscription.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
