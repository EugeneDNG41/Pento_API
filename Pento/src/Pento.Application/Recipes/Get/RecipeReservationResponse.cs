using Pento.Domain.FoodItemReservations;

namespace Pento.Application.Recipes.Get;

public sealed record RecipeReservationResponse(
    Guid Id,
    Guid FoodItemId,
    Guid HouseholdId,
    DateTime ReservationDateUtc,
    decimal Quantity,
    Guid UnitId,
    ReservationStatus Status,
    Guid RecipeId
);
