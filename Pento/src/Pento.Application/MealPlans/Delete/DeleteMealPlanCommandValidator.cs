using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.MealPlans.Delete;
internal sealed class DeleteMealPlanCommandValidator : AbstractValidator<DeleteMealPlanCommand>
{
    public DeleteMealPlanCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Meal plan Code is required.");
    }
}
