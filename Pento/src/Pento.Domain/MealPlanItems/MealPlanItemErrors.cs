using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.MealPlanItems;
public static class MealPlanItemErrors
{
    public static Error NotFound(Guid mealPlanItemId) =>
    Error.NotFound(
        "MealPlanItems.IdentityNotFound",
        $"The meal plan item with ID '{mealPlanItemId}' was not found."
    );

    public static Error NotFoundByMealPlan(Guid mealPlanId) =>
        Error.NotFound(
            "MealPlanItems.NotFoundByMealPlan",
            $"No meal plan items were found for meal plan '{mealPlanId}'."
        );

    public static readonly Error InvalidServings = Error.Problem(
        "MealPlanItems.InvalidServings",
        "The number of servings must be greater than zero."
    );

    public static readonly Error InvalidSchedule = Error.Problem(
        "MealPlanItems.InvalidSchedule",
        "The schedule for the meal plan item is invalid or empty."
    );

    public static readonly Error InvalidMealType = Error.Problem(
        "MealPlanItems.InvalidMealType",
        "The specified meal type is not valid."
    );

    public static readonly Error Conflict = Error.Conflict(
        "MealPlanItems.Conflict",
        "A meal plan item with the same recipe and schedule already exists in this meal plan."
    );

    public static readonly Error Unauthorized = Error.Problem(
        "MealPlanItems.Unauthorized",
        "You are not authorized to modify this meal plan item."
    );

    public static readonly Error InvalidDate = Error.Problem(
        "MealPlanItems.InvalidDate",
        "The scheduled date cannot be outside the meal plan duration."
    );
}
