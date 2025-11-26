using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.UpdatePlan;

internal sealed class UpdateSubscriptionPlanCommandHandler(
    IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateSubscriptionPlanCommand>
{
    public async Task<Result> Handle(UpdateSubscriptionPlanCommand command, CancellationToken cancellationToken)
    {
        SubscriptionPlan? plan = await subscriptionPlanRepository.GetByIdAsync(command.Id, cancellationToken);
        if (plan is null)
        {
            return Result.Failure(SubscriptionErrors.SubscriptionPlanNotFound);
        }
        Money? price = null;
        if (command.PriceAmount.HasValue || command.PriceCurrency != null)
        {
            long amount = command.PriceAmount ?? plan.Price.Amount;
            string currencyCode = command.PriceCurrency ?? plan.Price.Currency.Code;
            Result<Currency> currencyResult = Currency.FromCode(currencyCode);
            if (currencyResult.IsFailure)
            {
                return Result.Failure(currencyResult.Error);
            }
            price = new Money(amount, currencyResult.Value);
        }
        Duration? duration = null;
        if (command.DurationUnit.HasValue && command.DurationValue.HasValue)
        {
            duration = new Duration(command.DurationValue.Value, command.DurationUnit.Value);
        }
        bool duplicatePlan = await subscriptionPlanRepository
            .AnyAsync(sp => sp.Id != command.Id && sp.Price == price && sp.Duration == duration,
                cancellationToken);
        if (duplicatePlan)
        {
            return Result.Failure(SubscriptionErrors.DuplicateSubscriptionPlan);
        }
        plan.UpdateDetails(price, duration);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}


