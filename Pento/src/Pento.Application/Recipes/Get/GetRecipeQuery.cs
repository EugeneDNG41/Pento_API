using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Get;

public sealed record GetRecipeQuery(Guid RecipeId, string? Include)
    : IQuery<RecipeDetailResponse>;
