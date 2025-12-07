using FluentValidation;

namespace Pento.Application.RecipeIngredients.Update;

internal sealed class UpdateRecipeIngredientCommandValidator : AbstractValidator<UpdateRecipeIngredientCommand>
{
    public UpdateRecipeIngredientCommandValidator()
    {
        RuleFor(x => x.Notes)
            .NotEmpty().WithMessage("Notes is required.")
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Unit Id is required.");
    }
}
