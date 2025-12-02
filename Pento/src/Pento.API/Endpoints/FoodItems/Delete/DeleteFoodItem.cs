using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Delete;

internal sealed class DeleteFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("food-items/{foodItemId:guid}", async (
            Guid foodItemId,
            ICommandHandler<DeleteFoodItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeleteFoodItemCommand(foodItemId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Delete a specified food item");
    }
}
