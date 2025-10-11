using MediatR;
using Pento.API.Extensions;
using Pento.Application.Recipes.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes;

internal sealed class GetRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipes/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetRecipeQuery(id);

            Result<RecipeResponse> result = await sender.Send(query, cancellationToken);

            return result.Match(
                recipe => Results.Ok(recipe),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes);
    }
}
