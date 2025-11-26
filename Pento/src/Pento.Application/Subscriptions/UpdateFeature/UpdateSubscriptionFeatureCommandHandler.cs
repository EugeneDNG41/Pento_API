using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.UpdateFeature;

internal sealed class UpdateSubscriptionFeatureCommandHandler(
    IGenericRepository<Feature> featureRepository,
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSubscriptionFeatureCommand>
{
    public async Task<Result> Handle(UpdateSubscriptionFeatureCommand command, CancellationToken cancellationToken)
    {
        SubscriptionFeature? subscriptionFeature = await subscriptionFeatureRepository.GetByIdAsync(command.Id, cancellationToken);
        if (subscriptionFeature is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionFeatureNotFound);
        }
        Feature? feature = (await featureRepository
            .FindAsync(f => f.Name == command.FeatureName, cancellationToken)).SingleOrDefault();
        if (feature is null)
        {
            return Result.Failure(FeatureErrors.NotFound);
        }
        bool duplicateFeature = await subscriptionFeatureRepository
            .AnyAsync(sf => sf.Id != command.Id && sf.FeatureName == command.FeatureName,
                cancellationToken);
        if (duplicateFeature)
        {
            return Result.Failure(SubscriptionErrors.DuplicateSubscriptionFeature);
        }
        Limit? entitlement = command.EntitlementQuota.HasValue
            ? new Limit(command.EntitlementQuota.Value, command.EntitlementResetPer)
            : null;
        subscriptionFeature.UpdateDetails(feature.Name, entitlement);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}


