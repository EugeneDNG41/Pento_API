using FluentValidation;

namespace Pento.Application.Activities.Update;

internal sealed class UpdateActivityCommandValidator : AbstractValidator<UpdateActivityCommand>
{
    public UpdateActivityCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Activity Code is required.");
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
    }
}
