using MediatR;
using Pento.API.Extensions;
using Pento.Application.FoodReferences.Enrich;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class EnrichFoodReference : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/enrich", async (
            EnrichRequest request,
            IFoodAiEnricher enricher,
            CancellationToken cancellationToken) =>
        {
            FoodEnrichmentResult result = await enricher.EnrichAsync(
                new FoodEnrichmentAsk(
                    request.Name,
                    request.FoodGroup,
                    request.DataType
                ),
                cancellationToken
            );

            return Results.Ok(result);
        })
        .WithTags(Tags.FoodReferences);

    }

    internal sealed class EnrichRequest
    {
        public string Name { get; init; } = string.Empty;
        public string FoodGroup { get; init; } = string.Empty;
        public string DataType { get; init; } = "foundation_food";
    }
}
