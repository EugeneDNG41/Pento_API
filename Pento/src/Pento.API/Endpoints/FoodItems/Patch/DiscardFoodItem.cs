
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Discard;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class DiscardFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}/discard", async (
            Guid foodItemId,
            Request request,
            ICommandHandler<DiscardFoodItemCommand> handler,
            CancellationToken cancellationToken) =>
        {

            Result result = await handler.Handle(new DiscardFoodItemCommand(foodItemId, request.Quantity, request.UnitId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Discards a specific quantity of a food item");
    }
    internal sealed class Request
    {
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }
}
