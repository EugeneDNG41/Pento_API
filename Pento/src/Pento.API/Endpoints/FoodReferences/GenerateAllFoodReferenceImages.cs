using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.GenerateImage;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class GenerateAllFoodReferenceImages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/generate-all-images",
            async (
                int limit,
                ICommandHandler<GenerateAllFoodReferenceImagesCommand, int> handler,
                CancellationToken ct) =>
            {
                Result<int> result = await handler.Handle(
                    new GenerateAllFoodReferenceImagesCommand(limit),
                    ct);

                return result.Match(Results.Ok,
                    CustomResults.Problem
                );
            })
            .DisableAntiforgery()
            .WithTags(Tags.FoodReferences);
    }
}
