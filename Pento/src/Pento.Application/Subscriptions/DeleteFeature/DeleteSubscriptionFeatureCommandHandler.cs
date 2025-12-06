using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.DeleteFeature;

internal sealed class DeleteSubscriptionFeatureCommandHandler(
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteSubscriptionFeatureCommand>
{
    public async Task<Result> Handle(DeleteSubscriptionFeatureCommand command, CancellationToken cancellationToken)
    {
        SubscriptionFeature? subscriptionFeature = await subscriptionFeatureRepository.GetByIdAsync(command.Id, cancellationToken);
        if (subscriptionFeature is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionFeatureNotFound);
        }
        subscriptionFeature.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
