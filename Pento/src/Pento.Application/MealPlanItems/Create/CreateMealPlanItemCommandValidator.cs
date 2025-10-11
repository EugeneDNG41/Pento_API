using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.MealPlanItems.Create;
public sealed class CreateMealPlanItemCommandValidator : AbstractValidator<CreateMealPlanItemCommand>
{
    public CreateMealPlanItemCommandValidator()
    {
        RuleFor(x => x.MealPlanId)
            .NotEmpty()
            .WithMessage("MealPlanId is required.");

        RuleFor(x => x.RecipeId)
            .NotEmpty()
            .WithMessage("RecipeId is required.");

        RuleFor(x => x.MealType)
            .NotEmpty()
            .WithMessage("Meal type is required.");

        RuleFor(x => x.Schedule)
            .NotEmpty()
            .WithMessage("Schedule cannot be empty.")
            .Must(schedule => schedule.All(date => date > DateTime.MinValue))
            .WithMessage("All scheduled dates must be valid.");

        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be greater than zero.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
