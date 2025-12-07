using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeIngredients.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.RecipeIngredients.Get;

internal sealed class GetRecipeIngredientsByRecipeId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipes/{recipeId:guid}/ingredients", async (
            Guid recipeId,
            IQueryHandler<GetRecipeIngredientsByRecipeIdQuery, RecipeWithIngredientsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<RecipeWithIngredientsResponse> result = await handler.Handle(
                new GetRecipeIngredientsByRecipeIdQuery(recipeId), cancellationToken);

            return result.Match(
                ingredients => Results.Ok(ingredients),
                CustomResults.Problem);
        })
        .WithTags(Tags.RecipeIngredients);
    }
}
