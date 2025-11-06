
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
            [FromIfMatchHeader] string eTag,
            ICommandHandler<UpdateFoodItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateFoodItemCommand(
                foodItemId, 
                request.CompartmentId, 
                request.UnitId,
                request.Name,
                request.Quantity,
                request.ExpirationDate,
                request.Notes,
                ETagExtensions.ToExpectedVersion(eTag)), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization()
        .WithDescription("Update the attributes of a specified food item");
    }
    internal sealed class Request
    {
        public Guid CompartmentId { get; init; }
        public Guid UnitId { get; init; }
        public string? Name { get; init; }
        public decimal Quantity { get; init; }
        public DateTime ExpirationDate { get; init; }
        public string? Notes { get; init; }
    }
}
