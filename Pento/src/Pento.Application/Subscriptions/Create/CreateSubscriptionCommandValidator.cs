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
            .NotEmpty().WithMessage("Subscription description is required.")
            .MaximumLength(500).WithMessage("Subscription description must not exceed 500 characters.");
        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("Is Active is required.");
    }
}


