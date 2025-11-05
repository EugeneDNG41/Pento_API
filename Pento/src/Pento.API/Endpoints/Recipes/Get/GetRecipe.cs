using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Get;

internal sealed class GetRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipes/{recipeId:guid}", async (
            Guid recipeId,
            string? include,
            IQueryHandler<GetRecipeQuery, RecipeDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRecipeQuery(recipeId, include);

            Result<RecipeDetailResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                recipe => Results.Ok(recipe),
                CustomResults.Problem);
        })
        .WithTags(Tags.Recipes)
        .WithDescription("Include: 'None ,Ingredients, Directions, All' ");
    }
}
