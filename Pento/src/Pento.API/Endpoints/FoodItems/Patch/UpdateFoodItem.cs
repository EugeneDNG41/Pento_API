
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Patch;

internal sealed class UpdateFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("food-items/{foodItemId:guid}", async (
            Guid foodItemId,
            Request request,
            ICommandHandler<UpdateFoodItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateFoodItemCommand(
                foodItemId,
                request.StorageId,
                request.CompartmentId,
                request.UnitId,
                request.Name,
                request.Quantity,
                request.ExpirationDate,
                request.Notes), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Update the attributes of a specified food item");
    }
    internal sealed class Request
    {
        public Guid? StorageId { get; init; }
        public Guid? CompartmentId { get; init; }
        public Guid? UnitId { get; init; }
        public string? Name { get; init; }
        public decimal? Quantity { get; init; }
        public DateOnly? ExpirationDate { get; init; }
        public string? Notes { get; init; }
    }
}
