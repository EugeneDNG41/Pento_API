
using FluentValidation;

namespace Pento.Application.UserSubscriptions.CancelUserSubscription;

internal sealed class CancelUserSubscriptionCommandValidator : AbstractValidator<CancelUserSubscriptionCommand>
{
    public CancelUserSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User subscription Id is required.");
        RuleFor(x => x.Reason).MaximumLength(500).WithMessage("Cancellation reason cannot exceed 500 characters.");
    }
}

