using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.RecipeIngredients.Update;
internal sealed class UpdateRecipeIngredientCommandValidator : AbstractValidator<UpdateRecipeIngredientCommand>
{
    public UpdateRecipeIngredientCommandValidator()
    {
        RuleFor(x => x.Notes)
            .NotEmpty().WithMessage("Notes must not be empty.")
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("BaseQuantity must be greater than zero.");
        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Unit must not be empty.");
    }
}
