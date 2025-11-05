using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Get;

internal sealed class GetAllRecipes : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipes",
            async (IQueryHandler<GetAllRecipesQuery, IReadOnlyList<RecipeResponse>> handler, CancellationToken ct) =>
            {
                var query = new GetAllRecipesQuery();
                Result<IReadOnlyList<RecipeResponse>> result = await handler.Handle(query, ct);

                return result.Match(
                    recipes => Results.Ok(recipes),
                    CustomResults.Problem
                );
            })
            .WithTags(Tags.Recipes);
    }
}
