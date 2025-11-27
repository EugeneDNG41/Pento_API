using FluentValidation;

namespace Pento.Application.Subscriptions.Delete;

internal sealed class DeleteSubscriptionCommandValidator : AbstractValidator<DeleteSubscriptionCommand>
{
    public DeleteSubscriptionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Subscription Id is required.");
    }
}
