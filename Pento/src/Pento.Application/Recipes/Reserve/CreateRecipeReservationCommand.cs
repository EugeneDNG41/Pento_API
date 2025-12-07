using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Reserve;

public sealed record CreateRecipeReservationCommand(
    Guid FoodItemId,
    decimal Quantity,
    Guid UnitId,
    Guid RecipeId
) : ICommand<Guid>;
