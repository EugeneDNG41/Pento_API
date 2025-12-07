using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Features;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.UpdateFeature;
#pragma warning disable S125 
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

        if (!string.IsNullOrEmpty(command.FeatureCode))
        {
            Feature? feature = (await featureRepository.FindAsync(f => f.Code == command.FeatureCode, cancellationToken)).SingleOrDefault();
            if (feature is null)
            {
                return Result.Failure(FeatureErrors.NotFound);
            }
            bool duplicateFeature = await subscriptionFeatureRepository
            .AnyAsync(sf => sf.Id != command.Id && sf.FeatureCode == command.FeatureCode,
                cancellationToken);
            if (duplicateFeature)
            {
                return Result.Failure(SubscriptionErrors.DuplicateSubscriptionFeature);
            }
        }
        //bool hasTimedPlans = await subscriptionPlanRepository
        //    .AnyAsync(sp => sp.SubscriptionId == subscriptionFeature.SubscriptionId && sp.DurationInDays != null, cancellationToken);
        //if (hasTimedPlans && command.ResetPeriod == null)
        //{
        //    return Result.Failure<Guid>(SubscriptionErrors.CannotAddLifetimeFeatureToSubscriptionWithTimedPlans);
        //}
        subscriptionFeature.UpdateDetails(command.FeatureCode, command.Quota, command.ResetPeriod);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}


