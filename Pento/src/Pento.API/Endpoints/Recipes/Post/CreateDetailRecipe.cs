using Microsoft.AspNetCore.Mvc;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Post;

internal sealed class CreateDetailedRecipe : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipes/detailed", async (
                  [FromForm] CreateDetailedRecipeCommand request, 
                  ICommandHandler<CreateDetailedRecipeCommand, Guid> handler,
                  CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(request, cancellationToken);

            return result.Match(
                id => Results.Ok(new { RecipeId = id }),
                CustomResults.Problem);
        })
              .WithTags(Tags.Recipes)
              .RequireAuthorization()
              .DisableAntiforgery(); 
    }
}
