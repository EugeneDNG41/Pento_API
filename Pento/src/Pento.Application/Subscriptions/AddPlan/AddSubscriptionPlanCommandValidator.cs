using FluentValidation;

namespace Pento.Application.Subscriptions.AddPlan;

internal sealed class AddSubscriptionPlanCommandValidator : AbstractValidator<AddSubscriptionPlanCommand>
{
    public AddSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("Subscription Id is required.");
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.Currency)
            .NotNull().WithMessage("Currency is required.");
        RuleFor(x => x.DurationInDays)
            .GreaterThan(0).When(x => x.DurationInDays.HasValue)
            .WithMessage("Duration must be greater than zero.");
    }
}


