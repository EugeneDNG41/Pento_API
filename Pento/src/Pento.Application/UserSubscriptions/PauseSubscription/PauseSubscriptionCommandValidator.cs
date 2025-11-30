
using FluentValidation;

namespace Pento.Application.UserSubscriptions.PauseSubscription;

internal sealed class PauseSubscriptionCommandValidator : AbstractValidator<PauseSubscriptionCommand>
{
    public PauseSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User subscription Id is required.");
    }
}

