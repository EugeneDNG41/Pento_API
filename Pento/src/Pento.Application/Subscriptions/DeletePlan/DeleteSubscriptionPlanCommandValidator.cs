using FluentValidation;

namespace Pento.Application.Subscriptions.DeletePlan;

internal sealed class DeleteSubscriptionPlanCommandValidator : AbstractValidator<DeleteSubscriptionPlanCommand>
{
    public DeleteSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Subscription Plan Id is required.");
    }
}
