using FluentValidation;

namespace Pento.Application.RecipeIngredients.Delete;

internal sealed class DeleteRecipeIngredientCommandValidator : AbstractValidator<DeleteRecipeIngredientCommand>
{
    public DeleteRecipeIngredientCommandValidator()
    {
        RuleFor(x => x.RecipeIngredientId)
            .NotEmpty().WithMessage("Recipe ingredient Id is required.");
    }
}
