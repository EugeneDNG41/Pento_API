
using FluentValidation;

namespace Pento.Application.Users.CancelSubscription;

internal sealed class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User subscription Id is required.");
        RuleFor(x => x.Reason).MaximumLength(500).WithMessage("Cancellation reason cannot exceed 500 characters.");
    }
}

