using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    RecipeInfo? Recipe,
    FoodItemInfo? FoodItem
);

public sealed record RecipeInfo(
    Guid Id,
    string Title,
    string? Description,
    Uri? ImageUrl,
    int? Servings,
    string? DifficultyLevel
);

public sealed record FoodItemInfo(
    Guid Id,
    string Name,
    string FoodReferenceName,
    string FoodGroup,
    Uri? ImageUrl,
    decimal Quantity,
    string UnitAbbreviation,
    DateOnly ExpirationDate
);
