using FluentValidation;

namespace Pento.Application.Features.Update;

internal sealed class UpdateFeatureCommandValidator : AbstractValidator<UpdateFeatureCommand>
{
    public UpdateFeatureCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Feature Code is required.");
        RuleFor(x => x.Name)
            .NotEmpty().When(x => x.Name != null)
            .WithMessage("Name must not be empty if provided.");
        RuleFor(x => x.Name)
            .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Name must not exceed 100 characters.");
        RuleFor(x => x.Description)
            .NotEmpty().When(x => x.Description != null)
            .WithMessage("Description must not be empty if provided.");
        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Description must not exceed 500 characters.");
        RuleFor(x => x.DefaultQuota)
            .GreaterThan(0).When(x => x.DefaultResetPeriod != null)
            .WithMessage("Entitlement quota must be greater than zero when entitlement reset period is specified.");
    }
}
