using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Reserve;
public sealed record MealPlanReservationDetailResponse(
    MealPlanReservationInfo Reservation,
    FoodItemInfo FoodItem,
    MealPlanInfo MealPlan
);
public sealed record FoodItemInfo(
    Guid Id,
    string Name,
    decimal Quantity,
    Guid UnitId,
    DateOnly ExpirationDate,
    Uri? ImageUrl
);
public sealed record MealPlanInfo(
    Guid Id,
    string Name,
    MealType MealType,
    DateOnly ScheduledDate,
    int Servings,
    string? Notes
);

public sealed record MealPlanReservationInfo(
    Guid Id,
    Guid FoodItemId,
    Guid MealPlanId,
    decimal Quantity,
    Guid UnitId,
    DateTime ReservationDateUtc,
    ReservationStatus Status
);
