namespace Pento.Application.RecipeIngredients.Get;

public sealed record RecipeIngredientResponse(
    Guid Id,
    Guid RecipeId,
    Guid FoodRefId,
    decimal Quantity,
    Guid UnitId,
    string? Notes,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
