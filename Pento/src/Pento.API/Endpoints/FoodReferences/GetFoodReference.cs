using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class GetFoodReference : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-references/{id:guid}", async (Guid id, IQueryHandler<GetFoodReferenceQuery, FoodReferenceResponse> handler, CancellationToken cancellationToken) =>
        {
            var query = new GetFoodReferenceQuery(id);

            Result<FoodReferenceResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodReferences);
    }
}
