using MediatR;
using Pento.API.Endpoints;
using Pento.API.Extensions;
using Pento.Application.FoodReferences.GenerateImage;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class UploadFoodImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/{id:guid}/upload-image",
            async (Guid id, UploadRequest body, ISender sender, CancellationToken ct) =>
            {
                var cmd = new UploadFoodImageCommand(id, body.ImageUri);
                Result<string> result = await sender.Send(cmd, ct);

                return result.Match(
                    url => Results.Ok(new
                    {
                        Message = " Image uploaded successfully.",
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
