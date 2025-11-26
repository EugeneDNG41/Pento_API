using FluentValidation;

namespace Pento.Application.Subscriptions.Create;

internal sealed class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Subscription name is required.")
            .MaximumLength(20).WithMessage("Subscription name must not exceed 20 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Subscription description must not exceed 500 characters.");
    }
}


