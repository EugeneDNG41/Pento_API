using Pento.Domain.FoodItemReservations;

namespace Pento.Application.MealPlans.Reserve;

public sealed record MealPlanReservationResponse(
    Guid Id,
    Guid MealPlanId,

    string MealPlanName,
    string MealType,
    DateOnly ScheduledDate,
    int Servings,

    Guid FoodItemId,
    Guid FoodReferenceId,
    string FoodReferenceName,
    Uri? FoodReferenceImageUrl,
    string FoodGroup,
    decimal Quantity,
    string UnitAbbreviation,

    DateTime ReservationDateUtc,
    ReservationStatus Status
);

