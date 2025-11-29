
using FluentValidation;

namespace Pento.Application.Users.ResumeUserSubscription;

internal sealed class ResumeUserSubscriptionCommandValidator : AbstractValidator<ResumeUserSubscriptionCommand>
{
    public ResumeUserSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserSubscriptionId).NotEmpty().WithMessage("User subscription Id is required.");
    }
}

