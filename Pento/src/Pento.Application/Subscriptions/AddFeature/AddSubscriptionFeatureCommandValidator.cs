using FluentValidation;

namespace Pento.Application.Subscriptions.AddFeature;

internal sealed class AddSubscriptionFeatureCommandValidator : AbstractValidator<AddSubscriptionFeatureCommand>
{
    public AddSubscriptionFeatureCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty().WithMessage("Subscription Id is required.");
        RuleFor(x => x.FeatureCode)
            .NotEmpty().WithMessage("Feature Code is required.");
        RuleFor(x => x.Quota)
            .GreaterThan(0).When(x => x.ResetPeriod != null)
            .NotEmpty().When(x => x.ResetPeriod != null)
            .WithMessage("Quota must be greater than zero when reset period is specified.");
    }
}


