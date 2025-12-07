using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Update;

public sealed record UpdateRecipeCommand(
    Guid Id,
    string Title,
    string Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    string? Notes,
    int? Servings,
    DifficultyLevel? DifficultyLevel,
    Uri? ImageUrl,
    bool IsPublic
) : ICommand;
