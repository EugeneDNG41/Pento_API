using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Recipes.Get;
using Pento.Application.Recipes.Get.Public;
using Pento.Domain.Abstractions;
using Pento.Domain.Recipes;

namespace Pento.API.Endpoints.Recipes.Get;

internal sealed class GetPublicRecipes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("public/recipes", async (
            
            DifficultyLevel? difficultyLevel,
            string? search,
            string? sort,
            IQueryHandler<GetPublicRecipesQuery, PagedList<RecipeResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10
        ) =>
        {
            var query = new GetPublicRecipesQuery(
                PageNumber: pageNumber,
                PageSize: pageSize,
                DifficultyLevel: difficultyLevel,
                Search: search,
                Sort: sort
            );

            Result<PagedList<RecipeResponse>> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes);
    }
}
