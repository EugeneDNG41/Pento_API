using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.UpdatePlan;

internal sealed class UpdateSubscriptionPlanCommandHandler(
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IGenericRepository<SubscriptionFeature> subscriptionFeatureRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSubscriptionPlanCommand>
{
    public async Task<Result> Handle(UpdateSubscriptionPlanCommand command, CancellationToken cancellationToken)
    {
        SubscriptionPlan? plan = await subscriptionPlanRepository.GetByIdAsync(command.Id, cancellationToken);
        if (plan is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionPlanNotFound);
        }
        bool duplicatePlan = await subscriptionPlanRepository
            .AnyAsync(sp => sp.Id != command.Id && sp.Amount == command.Amount && sp.Currency == command.Currency && sp.DurationInDays == command.DurationInDays,
                cancellationToken);
        if (duplicatePlan)
        {
            return Result.Failure(SubscriptionErrors.DuplicateSubscriptionPlan);
        }
        bool lifetimeFeatureExists = await subscriptionFeatureRepository
            .AnyAsync(sf => sf.SubscriptionId == plan.SubscriptionId &&  sf.ResetPeriod == null, cancellationToken);
        if (lifetimeFeatureExists && command.DurationInDays != null)
        {
            return Result.Failure<Guid>(SubscriptionErrors.CannotAddTimedPlanToSubscriptionWithLifetimeFeatures);
        }
        bool lifetimePlanExists = await subscriptionPlanRepository
            .AnyAsync(sp => sp.Id != plan.Id && sp.SubscriptionId == plan.SubscriptionId && sp.DurationInDays == null, cancellationToken);
        if (lifetimeFeatureExists || lifetimePlanExists)
        {
            return Result.Failure<Guid>(SubscriptionErrors.NoMoreThanOneLifetimePlan);
        }
        
        plan.UpdateDetails(command.Amount, command.Currency, command.DurationInDays);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}


