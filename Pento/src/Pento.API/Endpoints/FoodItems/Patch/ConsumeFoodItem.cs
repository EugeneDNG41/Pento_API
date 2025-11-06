
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.AdjustQuantity;
using Pento.Application.FoodItems.Consume;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class ConsumeFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/consumption", async (
            Guid foodItemId,
            Request request,
            [FromIfMatchHeader] string eTag,
            ICommandHandler<ConsumeFoodItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ConsumeFoodItemCommand(foodItemId, request.Quantity, ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Consumes a specific quantity of a food item");
    }
    internal sealed class Request
    {
        public decimal Quantity { get; init; }
    }
}
