using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.AddPlan;
#pragma warning disable S125 
internal sealed class AddSubscriptionPlanCommandHandler(
    IGenericRepository<Subscription> subscriptionRepository,
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddSubscriptionPlanCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddSubscriptionPlanCommand command, CancellationToken cancellationToken)
    {
        Subscription? subscription = await subscriptionRepository.GetByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription is null)
        {
            return Result.Failure<Guid>(SubscriptionErrors.SubscriptionNotFound);
        }
        //bool lifetimeFeatureExists = await subscriptionFeatureRepository
        //    .AnyAsync(sf => sf.SubscriptionId == command.SubscriptionId && sf.ResetPeriod == null, cancellationToken);

        //if (lifetimeFeatureExists && command.DurationInDays != null)
        //{
        //    return Result.Failure<Guid>(SubscriptionErrors.CannotAddTimedPlanToSubscriptionWithLifetimeFeatures);
        //}
        bool lifetimePlanExists = await subscriptionPlanRepository
            .AnyAsync(sp => sp.SubscriptionId == command.SubscriptionId && sp.DurationInDays == null, cancellationToken);
        if (lifetimePlanExists)
        {
            return Result.Failure<Guid>(SubscriptionErrors.NoMoreThanOneLifetimePlan);
        }
        bool duplicatePlan = await subscriptionPlanRepository
            .AnyAsync(sp => sp.SubscriptionId == command.SubscriptionId &&
            sp.Amount == command.Amount &&
            sp.Currency == command.Currency &&
            sp.DurationInDays == command.DurationInDays, cancellationToken);
        if (duplicatePlan)
        {
            return Result.Failure<Guid>(SubscriptionErrors.DuplicateSubscriptionPlan);
        }
        var plan = SubscriptionPlan.Create(command.SubscriptionId, command.Amount, command.Currency, command.DurationInDays);
        subscriptionPlanRepository.Add(plan);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return subscription.Id; //return subscription id
    }
}


