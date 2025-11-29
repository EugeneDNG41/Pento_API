using FluentValidation;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Create.FromRecipe;

public sealed class CreateMealPlanFromRecipeConfirmCommandValidator
    : AbstractValidator<CreateMealPlanFromRecipeConfirmCommand>
{
    public CreateMealPlanFromRecipeConfirmCommandValidator()
    {
        RuleFor(x => x.RecipeId)
            .NotEmpty()
            .WithMessage("RecipeId is required.");

        RuleFor(x => x.MealType)
            .IsInEnum()
            .WithMessage("Invalid meal type value.");

        RuleFor(x => x.ScheduledDate)
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("ScheduledDate cannot be in the past.");

        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be greater than zero.")
            .LessThanOrEqualTo(20)
            .WithMessage("Servings is too large.");
    }
}
