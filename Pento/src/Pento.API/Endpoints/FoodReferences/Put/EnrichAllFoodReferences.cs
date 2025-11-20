using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Enrich;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences.Put;

internal sealed class EnrichAllFoodReferences : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-references/auto-enrich-shelf-life", async (
                ICommandHandler<EnrichAllShelfLifeCommand> handler,
                CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new EnrichAllShelfLifeCommand(), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodReferences);
    }
}
