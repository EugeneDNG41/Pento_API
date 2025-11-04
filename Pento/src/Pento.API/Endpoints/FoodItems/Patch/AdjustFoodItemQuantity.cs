
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.AdjustQuantity;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class AdjustFoodItemQuantity : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/quantity", async (
            Guid foodItemId,
            decimal quantity,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<AdjustFoodItemQuantityCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new AdjustFoodItemQuantityCommand(foodItemId, quantity, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Adjusts the quantity of a specific food item by a given amount");
    }
}
