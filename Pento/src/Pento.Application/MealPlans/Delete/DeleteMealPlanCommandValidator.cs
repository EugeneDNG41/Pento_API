using FluentValidation;

namespace Pento.Application.MealPlans.Delete;

internal sealed class DeleteMealPlanCommandValidator : AbstractValidator<DeleteMealPlanCommand>
{
    public DeleteMealPlanCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Meal plan Id is required.");
    }
}
