using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.MealPlans.Create;
public sealed class CreateMealPlanCommandValidator : AbstractValidator<CreateMealPlanCommand>
{
    public CreateMealPlanCommandValidator()
    {
        RuleFor(x => x.HouseholdId)
            .NotEmpty().WithMessage("HouseholdId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Meal plan name cannot be empty.")
            .MaximumLength(100).WithMessage("Meal plan name cannot exceed 100 characters.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty().WithMessage("CreatedBy is required.");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("Start date must not be after end date.");
    }
}
