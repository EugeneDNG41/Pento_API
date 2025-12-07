using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.GenerateImage;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class UploadFoodImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/{id:guid}/upload-image",
           async (
                Guid id,
                UploadRequest body,
                ICommandHandler<UploadFoodImageCommand, string> handler,
                CancellationToken ct) =>
           {
               var cmd = new UploadFoodImageCommand(id, body.ImageUri);
               Result<string> result = await handler.Handle(cmd, ct);

               return result.Match(
                   url => Results.Ok(new
                   {
                       Message = " ImageUrl uploaded successfully.",
                       ImageUrl = url
                   }),
                   error => CustomResults.Problem(error)
               );
           })
            .DisableAntiforgery()
            .WithTags(Tags.FoodReferences);
    }

    internal sealed record UploadRequest(Uri ImageUri);
}
