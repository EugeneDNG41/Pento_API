using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeDirections.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.API.Endpoints.RecipeDirections.Get;

internal sealed class GetRecipeDirectionsByRecipeId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipes/{recipeId:guid}/directions", async (
            Guid recipeId,
            IQueryHandler<GetRecipeDirectionsByRecipeIdQuery, RecipeWithDirectionsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<RecipeWithDirectionsResponse> result = await handler.Handle(
                new GetRecipeDirectionsByRecipeIdQuery(recipeId), cancellationToken);

            return result.Match(
                directions => Results.Ok(directions),
                CustomResults.Problem);
        })
        .WithTags(Tags.RecipeDirections);
    }
}
