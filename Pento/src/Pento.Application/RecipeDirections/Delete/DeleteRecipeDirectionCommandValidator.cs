using FluentValidation;

namespace Pento.Application.RecipeDirections.Delete;

internal sealed class DeleteRecipeDirectionCommandValidator : AbstractValidator<DeleteRecipeDirectionCommand>
{
    public DeleteRecipeDirectionCommandValidator()
    {
        RuleFor(x => x.RecipeDirectionId)
            .NotEmpty()
            .WithMessage("Recipe direction Id is required.");
    }
}
