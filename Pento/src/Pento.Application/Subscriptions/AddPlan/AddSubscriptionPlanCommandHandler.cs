using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.AddPlan;

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
        Duration? duration = command.DurationUnit.HasValue && command.DurationValue.HasValue
            ? new Duration(command.DurationValue.Value, command.DurationUnit.Value)
            : null;
        Result<Currency> currencyResult = Currency.FromCode(command.PriceCurrency);
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Guid>(currencyResult.Error);
        }
        var price = new Money(command.PriceAmount, currencyResult.Value);
        bool duplicatePlan = await subscriptionPlanRepository
            .AnyAsync(sp => sp.SubscriptionId == command.SubscriptionId&& sp.Price == price&& sp.Duration == duration,
                cancellationToken);
        if (duplicatePlan)
        {
            return Result.Failure<Guid>(SubscriptionErrors.DuplicateSubscriptionPlan);
        }
        var plan = SubscriptionPlan.Create(command.SubscriptionId, price, duration);
        subscriptionPlanRepository.Add(plan);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return subscription.Id; //return subscription id
    }
}


