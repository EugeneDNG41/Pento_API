using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeDirections.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.RecipeDirections.Put;

internal sealed class UpdateRecipeDirectionImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("recipes-directions/{directionId:guid}/image", async (
            Guid recipeDirectionId,
            IFormFile file,
            ICommandHandler<UpdateRecipeDirectionImageCommand, string> handler,
            CancellationToken ct) =>
        {
            var cmd = new UpdateRecipeDirectionImageCommand(recipeDirectionId, file);

            Result<string> result = await handler.Handle(cmd, ct);

            return result.Match(
                url => Results.Ok(new
                {
                    Message = "Direction image updated successfully.",
                    ImageUrl = url
                }),
                CustomResults.Problem
            );
        })
        .DisableAntiforgery()
        .RequireAuthorization()
        .WithTags(Tags.RecipeDirections);
    }
}
