using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeIngredients.Create;

public sealed record CreateRecipeIngredientCommand(
    Guid RecipeId,
    Guid FoodRefId,
    decimal Quantity,
    Guid UnitId,
    string? Notes
) : ICommand<Guid>;
