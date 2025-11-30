using FluentValidation;

namespace Pento.Application.UserSubscriptions.AdjustUserSubscription;

internal sealed class AdjustUserSubscriptionCommandValidator : AbstractValidator<AdjustUserSubscriptionCommand>
{
    public AdjustUserSubscriptionCommandValidator()
    {
        RuleFor(x => x.DurationInDays)
            .NotEqual(0)
            .WithMessage("Duration in days must be non-zero.");
    }
}

