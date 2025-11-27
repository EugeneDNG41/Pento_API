using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.Create;

internal sealed class CreateSubscriptionCommandHandler(
    IGenericRepository<Subscription> subscriptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateSubscriptionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        bool nameTaken = await subscriptionRepository
            .AnyAsync(s => s.Name == command.Name, cancellationToken);
        if (nameTaken)
        {
            return Result.Failure<Guid>(SubscriptionErrors.NameTaken);
        }
        var subscription = Subscription.Create(command.Name, command.Description, command.IsActive);
        subscriptionRepository.Add(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return subscription.Id;
    }
}


