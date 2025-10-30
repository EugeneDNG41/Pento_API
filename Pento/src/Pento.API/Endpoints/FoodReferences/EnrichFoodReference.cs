using Pento.API.Extensions;
using Pento.Application.FoodReferences.Enrich;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class EnrichFoodReference : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/{id:guid}/auto-enrich-shelf-life",
            async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var cmd = new EnrichFoodReferenceShelfLifeCommand(id);

                Result<FoodEnrichmentResult> result = await sender.Send(cmd, cancellationToken);

                return result.Match(
                    value => Results.Ok(new
                    {
                        FoodReferenceId = id,
                        Updated = true,
                        ShelfLife = new
                        {
                            Pantry = value.TypicalShelfLifeDays_Pantry,
                            Fridge = value.TypicalShelfLifeDays_Fridge,
                            Freezer = value.TypicalShelfLifeDays_Freezer
                        }
                    }),
                    error => CustomResults.Problem(error)
                );
            })
            .WithTags(Tags.FoodReferences);
    }
}
