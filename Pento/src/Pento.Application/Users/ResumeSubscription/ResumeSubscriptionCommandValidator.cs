
using FluentValidation;

namespace Pento.Application.Users.ResumeSubscription;

internal sealed class ResumeSubscriptionCommandValidator : AbstractValidator<ResumeSubscriptionCommand>
{
    public ResumeSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User subscription Id is required.");
    }
}

