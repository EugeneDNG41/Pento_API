using FluentValidation;

namespace Pento.Application.DietaryTags.Create;

public sealed class CreateDietaryTagCommandValidator : AbstractValidator<CreateDietaryTagCommand>
{
    public CreateDietaryTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Dietary tag name is required.")
            .MaximumLength(100).WithMessage("Dietary tag name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
