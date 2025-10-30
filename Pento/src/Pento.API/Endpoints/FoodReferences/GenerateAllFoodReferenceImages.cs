using MediatR;
using Pento.API.Endpoints;
using Pento.API.Extensions;
using Pento.Application.FoodReferences.GenerateImage;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class GenerateAllFoodReferenceImages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/generate-all-images",
            async (int limit, ISender sender, CancellationToken ct) =>
            {
                Result<int> result = await sender.Send(new GenerateAllFoodReferenceImagesCommand(limit), ct);

                return result.Match(
                    count => Results.Ok(new
                    {
                        Message = $" Generated {count} images (requested {limit}, max 50 per call).",
                        GeneratedCount = count
                    }),
                    error => CustomResults.Problem(error)
                );
            })
            .DisableAntiforgery()
            .WithTags(Tags.FoodReferences);
    }
}
