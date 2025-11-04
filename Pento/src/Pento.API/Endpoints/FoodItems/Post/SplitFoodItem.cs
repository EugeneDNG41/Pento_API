using Azure;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Split;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Post;

internal sealed class SplitFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-items/{foodItemId:guid}/split", async (
            Guid foodItemId,
            decimal quantity,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<SplitFoodItemCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(new SplitFoodItemCommand(foodItemId, quantity, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(guid => Results.CreatedAtRoute(RouteNames.GetFoodItemById, new {id = guid}, new { id = guid }), CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Splits a specific quantity from a food item into a new food item");
    }

}
