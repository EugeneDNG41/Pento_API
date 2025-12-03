using System;

namespace Pento.Application.Recipes.Get;

public sealed class RecipeDetailResponse
{
    public Guid RecipeId { get; init; }
    public string RecipeTitle { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int PrepTimeMinutes { get; init; }
    public int CookTimeMinutes { get; init; }
    public int TotalTimes { get; init; }
    public string? Notes { get; init; }
    public int? Servings { get; init; }
    public string? DifficultyLevel { get; init; }
    public string? ImageUrl { get; init; }
    public Guid CreatedBy { get; init; }
    public bool IsPublic { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime UpdatedOnUtc { get; init; }

    public List<RecipeIngredientItem> Ingredients { get; init; } = [];
    public List<RecipeDirectionItem> Directions { get; init; } = [];
}

public sealed class RecipeIngredientItem
{
    public Guid IngredientId { get; init; }
    public Guid FoodRefId { get; init; }
    public string FoodRefName { get; init; } = string.Empty;
    public Uri ImageUrl {  get; init; }
    public decimal Quantity { get; init; }
    public Guid UnitId { get; init; }
    public string UnitName { get; init; } = string.Empty;
    public string? Notes { get; init; }
}

public sealed class RecipeDirectionItem
{
    public Guid DirectionId { get; init; }
    public int StepNumber { get; init; }
    public string Description { get; init; } = string.Empty;
    public Uri? ImageUrl { get; init; }
}
