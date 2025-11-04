using FluentValidation;

namespace Pento.Application.RecipeDirections.Update;

internal sealed class UpdateRecipeDirectionCommandValidator : AbstractValidator<UpdateRecipeDirectionCommand>
{
    public UpdateRecipeDirectionCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

    }
}
