
using FluentValidation;

namespace Pento.Application.Users.PauseUserSubscription;

internal sealed class PauseUserSubscriptionCommandValidator : AbstractValidator<PauseUserSubscriptionCommand>
{
    public PauseUserSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User subscription Id is required.");
    }
}

