using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlans;
public static class MealPlanErrors
{
    public static Error NotFound(Guid mealPlanId) =>
        Error.NotFound(
            "MealPlans.NotFound",
            $"The meal plan with ID {mealPlanId} was not found."
        );

    public static readonly Error InvalidName = Error.Problem(
        "MealPlans.InvalidName",
        "The meal plan name cannot be empty or whitespace."
    );

    public static readonly Error InvalidDuration = Error.Problem(
        "MealPlans.InvalidDuration",
        "The meal plan duration is invalid or overlaps with another plan."
    );

    public static readonly Error Conflict = Error.Conflict(
        "MealPlans.Conflict",
        "A meal plan with the same name already exists for this household."
    );

    public static readonly Error Unauthorized = Error.Problem(
        "MealPlans.Unauthorized",
        "You are not authorized to modify this meal plan."
    );
    public static readonly Error DuplicateName = Error.Conflict(
    "MealPlans.DuplicateName",
    "A meal plan with the same name already exists for this household."
);

    public static readonly Error InvalidDateRange = Error.Problem(
        "MealPlans.InvalidDateRange",
        "Start date cannot be later than end date."
    );
}
