using FluentValidation;

namespace Pento.Application.MealPlans.Create;

public sealed class CreateMealPlanCommandValidator : AbstractValidator<CreateMealPlanCommand>
{
    public CreateMealPlanCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Meal plan name is required.")
            .MaximumLength(100).WithMessage("Meal plan name cannot exceed 100 characters.");
        RuleFor(x => x.Servings)
            .GreaterThan(0).WithMessage("Servings must be greater than zero.");
        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("Scheduled date is required.");
        RuleFor(x => x.MealType)
    .IsInEnum()
    .WithMessage("Meal type must be one of: Breakfast, Lunch, Dinner, or Snack.");
        RuleFor(x => x)
    .Must(x => x.RecipeId is not null || x.FoodItemId is not null)
    .WithMessage("You must specify either a recipe or a food item.");
        RuleFor(x => x)
    .Must(x => !(x.RecipeId is not null && x.FoodItemId is not null))
    .WithMessage("Meal plan can only reference either a recipe or a food item, not both.");

    }

}

