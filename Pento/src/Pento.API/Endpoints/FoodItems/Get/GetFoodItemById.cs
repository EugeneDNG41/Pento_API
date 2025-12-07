using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Get;

internal sealed class GetFoodItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-items/{id:guid}", async (
            Guid id,
            IQueryHandler<GetFoodItemByIdQuery, FoodItemDetail> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFoodItemByIdQuery(id);

            Result<FoodItemDetail> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                response => Results.Ok(response),
                CustomResults.Problem
            );
        })
        .WithName(RouteNames.GetFoodItemById)
        .RequireAuthorization()
        .WithTags(Tags.FoodItems);
    }
}
