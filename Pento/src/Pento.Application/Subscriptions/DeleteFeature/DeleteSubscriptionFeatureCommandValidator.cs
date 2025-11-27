using FluentValidation;

namespace Pento.Application.Subscriptions.DeleteFeature;

internal sealed class DeleteSubscriptionFeatureCommandValidator : AbstractValidator<DeleteSubscriptionFeatureCommand>
{
    public DeleteSubscriptionFeatureCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Subscription Feature Id is required.");
    }
}
