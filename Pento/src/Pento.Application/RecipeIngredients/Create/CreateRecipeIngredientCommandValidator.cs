using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.RecipeIngredients.Create;
public sealed class CreateRecipeIngredientCommandValidator
    : AbstractValidator<CreateRecipeIngredientCommand>
{
    public CreateRecipeIngredientCommandValidator()
    {
        RuleFor(x => x.RecipeId)
            .NotEmpty().WithMessage("RecipeId is required.");

        RuleFor(x => x.FoodRefId)
            .NotEmpty().WithMessage("FoodRefId is required.");

        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("UnitId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
