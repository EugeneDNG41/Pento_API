using FluentValidation;

namespace Pento.Application.Subscriptions.AddPlan;

internal sealed class AddSubscriptionPlanCommandValidator : AbstractValidator<AddSubscriptionPlanCommand>
{
    public AddSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("Subscription Id is required.");
        RuleFor(x => x.PriceAmount)
            .GreaterThan(0).WithMessage("Price amount must be greater than zero.");
        RuleFor(x => x.PriceCurrency)
            .NotEmpty().WithMessage("Price currency is required.")
            .Length(3).WithMessage("Price currency must be a valid 3-letter ISO currency code.");
        RuleFor(x => x.DurationValue)
            .GreaterThan(0).When(x => x.DurationUnit != null)
            .WithMessage("Duration value must be greater than zero when duration unit is specified.");
        RuleFor(x => x.DurationUnit)
            .NotNull().When(x => x.DurationValue != null)
            .WithMessage("Duration unit is required when duration value is specified.");

    }
}


