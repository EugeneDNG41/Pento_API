using FluentValidation;

namespace Pento.Application.Subscriptions.UpdateFeature;

internal sealed class UpdateSubscriptionFeatureCommandValidator : AbstractValidator<UpdateSubscriptionFeatureCommand>
{
    public UpdateSubscriptionFeatureCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Subscription Feature Id is required.");
        RuleFor(x => x.FeatureCode)
            .MaximumLength(50).WithMessage("Feature Code must not exceed 50 characters.")
            .When(x => x.FeatureCode != null);
        RuleFor(x => x.Quota)
            .GreaterThan(0).When(x => x.ResetPeriod != null)
            .NotEmpty().When(x => x.ResetPeriod != null)
            .WithMessage("Quota must be greater than zero when reset period is specified.");
    }
}


