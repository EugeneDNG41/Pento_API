
using FluentValidation;

namespace Pento.Application.UserSubscriptions.PauseUserSubscription;

internal sealed class PauseUserSubscriptionCommandValidator : AbstractValidator<PauseUserSubscriptionCommand>
{
    public PauseUserSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User subscription Id is required.");
    }
}

