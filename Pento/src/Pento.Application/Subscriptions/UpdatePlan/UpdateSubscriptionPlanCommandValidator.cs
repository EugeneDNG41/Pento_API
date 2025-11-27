using FluentValidation;

namespace Pento.Application.Subscriptions.UpdatePlan;

internal sealed class UpdateSubscriptionPlanCommandValidator : AbstractValidator<UpdateSubscriptionPlanCommand>
{
    public UpdateSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subscription Plan Code is required.");
        RuleFor(x => x.Amount)
            .GreaterThan(0).When(x => x.Amount.HasValue)
            .WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.DurationInDays)
            .GreaterThan(0).When(x => x.DurationInDays.HasValue)
            .WithMessage("Duration must be greater than zero.");

    }
}


