using Pento.Domain.FoodItemReservations;

namespace Pento.Application.Recipes.Get;

public sealed record RecipeReservationDetailResponse(
    ReservationInfo Reservation,
    FoodItemInfo FoodItem,
    RecipeInfo Recipe
);

public sealed record ReservationInfo(
    Guid Id,
    Guid FoodItemId,
    Guid HouseholdId,
    DateTime ReservationDateUtc,
    decimal Quantity,
    Guid UnitId,
    ReservationStatus Status,
    Guid RecipeId
);

public sealed record FoodItemInfo(
    Guid Id,
    string Name,
    decimal Quantity,
    Guid UnitId,
    DateTime? ExpirationDate,
    Uri? ImageUrl,
    string? Notes
);

public sealed record RecipeInfo(
    Guid Id,
    string Title,
    string? Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    Uri? ImageUrl,
    Guid CreatedBy
);
