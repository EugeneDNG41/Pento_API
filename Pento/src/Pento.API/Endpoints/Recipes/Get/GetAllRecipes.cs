using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Recipes.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.API.Endpoints.Recipes.Get;

internal sealed class GetAllRecipes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipes", async (
    int pageNumber,
    int pageSize,
    DifficultyLevel? difficulty,
    IQueryHandler<GetAllRecipesQuery, PagedList<RecipeResponse>> handler,
    CancellationToken cancellationToken
) =>
        {
            var query = new GetAllRecipesQuery(
                PageNumber: pageNumber,
                PageSize: pageSize,
                DifficultyLevel: difficulty
            );

            Result<PagedList<RecipeResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes);
    }
}
