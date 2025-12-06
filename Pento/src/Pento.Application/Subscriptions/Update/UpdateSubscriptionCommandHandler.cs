using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.Update;

internal sealed class UpdateSubscriptionCommandHandler(
    IGenericRepository<Subscription> subscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSubscriptionCommand>
{
    public async Task<Result> Handle(UpdateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (subscription is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionNotFound);
        }
        if (command.Name != null && subscription.Name != command.Name)
        {
            bool nameTaken = await subscriptionRepository
                .AnyAsync(s => s.Name == command.Name && s.Id != command.Id, cancellationToken);
            if (nameTaken)
            {
                return Result.Failure(SubscriptionErrors.NameTaken);
            }
        }
        subscription.UpdateDetails(command.Name, command.Description, command.IsActive);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}


