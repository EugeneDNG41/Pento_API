using FluentValidation;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Update;

internal sealed class UpdateMealPlanCommandValidator : AbstractValidator<UpdateMealPlanCommand>
{
    public UpdateMealPlanCommandValidator()
    {
        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be greater than 0.");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty()
            .WithMessage("ScheduledDate is required.");

        RuleFor(x => x.MealType)
    .IsInEnum()
    .WithMessage("Meal type must be one of: Breakfast, Lunch, Dinner, or Snack.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
