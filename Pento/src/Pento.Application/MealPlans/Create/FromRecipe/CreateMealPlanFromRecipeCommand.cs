using Pento.Application.Abstractions.Messaging;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Create.FromRecipe;

public sealed record CreateMealPlanFromRecipeCommand(
    Guid RecipeId,
    MealType MealType,
    DateOnly ScheduledDate,
    int Servings
) : ICommand<MealPlanAutoReserveResult>;

public sealed record MealPlanAutoReserveResult(
    Guid MealPlanId,
    IReadOnlyList<ReservationResult> Reservations,
    IReadOnlyList<MissingIngredientResult> Missing

);

public sealed record ReservationResult(
    Guid FoodItemId,
    Guid IngredientId,
    decimal ReservedQuantity,
    decimal IngredientQuantity,
    Guid IngredientUnitId,
    Guid FoodItemUnitId,
    string IngredientUnitAbbreviation,
    string FoodItemUnitAbbreviation
);

public sealed record MissingIngredientResult(
    Guid IngredientId,
    Guid FoodRefId,
    string Name,
    decimal RequiredQuantity,
    Guid UnitId,
    string UnitAbbreviation
);
