using FluentValidation;

namespace Pento.Application.Subscriptions.AddFeature;

internal sealed class AddSubscriptionFeatureCommandValidator : AbstractValidator<AddSubscriptionFeatureCommand>
{
    public AddSubscriptionFeatureCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("Subscription Id is required.");
        RuleFor(x => x.FeatureName)
            .NotEmpty().WithMessage("Feature name is required.")
            .MaximumLength(50).WithMessage("Feature name must not exceed 100 characters.");
        RuleFor(x => x.EntitlementQuota)
            .GreaterThan(0).When(x => x.EntitlementResetPer != null)
            .WithMessage("Entitlement quota must be greater than zero when entitlement reset period is specified.");
    }
}


