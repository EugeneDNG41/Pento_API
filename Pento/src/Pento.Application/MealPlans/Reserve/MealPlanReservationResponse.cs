using Pento.Domain.FoodItemReservations;

namespace Pento.Application.MealPlans.Reserve;

public sealed record MealPlanReservationResponse(
    Guid Id,
    Guid FoodItemId,
    Guid MealPlanId,
    decimal Quantity,
    Guid UnitId,
    DateTime ReservationDateUtc,
    ReservationStatus Status
);
