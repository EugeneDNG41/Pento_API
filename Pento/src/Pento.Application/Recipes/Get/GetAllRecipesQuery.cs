using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Get;

public sealed record GetAllRecipesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    DifficultyLevel? DifficultyLevel = null,
    string? Search = null,
    string? Sort = "newest" 
) : IQuery<PagedList<RecipeResponse>>;
