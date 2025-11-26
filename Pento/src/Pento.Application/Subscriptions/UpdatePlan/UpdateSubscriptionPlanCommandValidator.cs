using FluentValidation;

namespace Pento.Application.Subscriptions.UpdatePlan;

internal sealed class UpdateSubscriptionPlanCommandValidator : AbstractValidator<UpdateSubscriptionPlanCommand>
{
    public UpdateSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subscription Plan Id is required.");
        RuleFor(x => x.PriceAmount)
            .GreaterThan(0).When(x => x.PriceAmount.HasValue)
            .WithMessage("Price amount must be greater than zero.");
        RuleFor(x => x.PriceCurrency)
            .Length(3).When(x => x.PriceCurrency != null)
            .WithMessage("Price currency must be a valid 3-letter ISO currency code.");
        RuleFor(x => x.DurationValue)
            .GreaterThan(0).When(x => x.DurationUnit != null && x.DurationValue.HasValue)
            .WithMessage("Duration value must be greater than zero when duration unit is specified.");
        RuleFor(x => x.DurationUnit)
            .NotNull().When(x => x.DurationValue != null)
            .WithMessage("Duration unit is required when duration value is specified.");
    }
}


