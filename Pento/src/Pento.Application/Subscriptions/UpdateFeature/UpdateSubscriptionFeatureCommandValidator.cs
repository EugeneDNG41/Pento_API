using FluentValidation;

namespace Pento.Application.Subscriptions.UpdateFeature;

internal sealed class UpdateSubscriptionFeatureCommandValidator : AbstractValidator<UpdateSubscriptionFeatureCommand>
{
    public UpdateSubscriptionFeatureCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subscription Feature Id is required.");
        RuleFor(x => x.FeatureName)
            .MaximumLength(50).WithMessage("Feature name must not exceed 100 characters.");
        RuleFor(x => x.EntitlementQuota)
            .GreaterThan(0).When(x => x.EntitlementResetPer != null)
            .WithMessage("Entitlement quota must be greater than zero when entitlement reset period is specified.");
    }
}


