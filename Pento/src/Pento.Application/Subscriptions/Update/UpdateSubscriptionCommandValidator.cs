using FluentValidation;

namespace Pento.Application.Subscriptions.Create;

internal sealed class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subscription Id is required.");
        RuleFor(x => x.Name)
            .MaximumLength(20).WithMessage("Subscription name must not exceed 20 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Subscription description must not exceed 500 characters.");
    }
}


