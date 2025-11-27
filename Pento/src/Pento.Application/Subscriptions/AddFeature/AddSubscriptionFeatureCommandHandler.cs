using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.AddFeature;

internal sealed class AddSubscriptionFeatureCommandHandler(
    IGenericRepository<Feature> featureRepository,
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddSubscriptionFeatureCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddSubscriptionFeatureCommand command, CancellationToken cancellationToken)
    {
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
        {
            return Result.Failure<Guid>(SubscriptionErrors.SubscriptionNotFound);
        }
        Feature? feature = (await featureRepository.FindAsync(f => f.Code == command.FeatureCode, cancellationToken)).SingleOrDefault();
        if (feature is null)
        {
            return Result.Failure<Guid>(FeatureErrors.NotFound);
        }
        
        bool duplicateFeature = await subscriptionFeatureRepository
            .AnyAsync(sf => sf.SubscriptionId == command.SubscriptionId && sf.FeatureCode == command.FeatureCode,
                cancellationToken);
        if (duplicateFeature)
        {
            return Result.Failure<Guid>(SubscriptionErrors.DuplicateSubscriptionFeature);
        }
        bool hasTimedPlans = await subscriptionPlanRepository
            .AnyAsync(sp => sp.SubscriptionId == command.SubscriptionId && sp.DurationInDays != null, cancellationToken);
        if (hasTimedPlans && command.ResetPeriod == null)
        {
            return Result.Failure<Guid>(SubscriptionErrors.CannotAddLifetimeFeatureToSubscriptionWithTimedPlans);
        }
        var subscriptionFeature = SubscriptionFeature.Create(
            command.SubscriptionId,
            feature.Code,
            command.Quota,
            command.ResetPeriod);
        subscriptionFeatureRepository.Add(subscriptionFeature);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return subscription.Id;
    }
}


