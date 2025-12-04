using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Recipes.Get;
using Pento.Domain.FoodItemReservations;

namespace Pento.Application.MealPlans.Get;

public sealed record MealPlanDetailResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    DateOnly ScheduledDate,
    string MealType,
    int Servings,
    string? Notes,
    Guid CreatedBy,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc,
    IReadOnlyList<MealPlanRecipeInfo> Recipes,
    IReadOnlyList<MealPlanFoodItemInfo> FoodItems
);

public sealed record MealPlanRecipeInfo(
    Guid Id,
    string Title,
    string? Description,
    Uri? ImageUrl,
    int? Servings,
    string? DifficultyLevel
);

public sealed record MealPlanFoodItemInfo(
    Guid Id,
    Guid ReservationId,
    string Name,
    string FoodReferenceName,
    string FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    ReservationStatus Status,
    DateOnly ExpirationDate
);



