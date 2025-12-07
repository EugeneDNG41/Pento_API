using Microsoft.AspNetCore.Mvc;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Put;

internal sealed class UpdateRecipeImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("recipes/{id:guid}/image", async (
            Guid id,
            [FromForm] Request request,
            ICommandHandler<UpdateRecipeImageCommand, string> handler,
            CancellationToken ct) =>
        {
            var cmd = new UpdateRecipeImageCommand(id, request.ImageFile);
            Result<string> result = await handler.Handle(cmd, ct);

            return result.Match(
                url => Results.Ok(new
                {
                    Message = "Recipe image updated.",
                    ImageUrl = url
                }),
                CustomResults.Problem
            );
        })
        .DisableAntiforgery()
        .RequireAuthorization()
        .WithTags(Tags.Recipes);
    }

    internal sealed class Request
    {
        public IFormFile ImageFile { get; init; }
    }
}
