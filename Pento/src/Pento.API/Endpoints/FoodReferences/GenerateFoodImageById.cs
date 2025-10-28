using Pento.API.Extensions;
using Pento.Application.FoodReferences.GenerateImage;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class GenerateFoodImageById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/{id:guid}/generate-image",
        async (Guid id, IFoodImageGenerator generator, CancellationToken ct) =>
        {
            string? base64 = await generator.GenerateImageAsync(id, ct);
            if (base64 is null)
            {
                return Results.NotFound($"FoodReference {id} not found or image generation failed.");
            }

            return Results.Ok(new
            {
                FoodId = id,
                Base64 = base64
            });
        })
        .WithTags(Tags.FoodReferences);
    }
}
