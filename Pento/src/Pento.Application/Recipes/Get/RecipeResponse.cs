namespace Pento.Application.Recipes.Get;

public sealed record RecipeResponse(
    Guid Id,
    string Title,
    string? Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    int TotalTimes,
    string? Notes,
    int? Servings,
    string? DifficultyLevel,
    Uri? ImageUrl,
    Guid CreatedBy,
    bool IsPublic,
    DateTime CreatedOnUtc,
    DateTime UpdatedOnUtc
);
