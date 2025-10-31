using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Get;

internal sealed class GetFoodItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-items/{id:guid}", async (
            Guid id, 
            IQueryHandler<GetFoodItemQuery, FoodItemResponse> handler, 
            CancellationToken cancellationToken) =>
        {
            var query = new GetFoodItemQuery(id);

            Result<FoodItemResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                response => Results.Ok(response),
                CustomResults.Problem
            );
        })
        .WithName("GetFoodItemById")
        .RequireAuthorization()
        .WithTags(Tags.FoodItems);
    }
}
