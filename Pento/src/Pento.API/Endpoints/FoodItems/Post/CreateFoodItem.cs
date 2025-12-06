using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.FoodItems.Post;

internal sealed class CreateFoodItemBulk : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-items/bulk", async (
            List<CreateFoodItem.Request> requests,
            ICommandHandler<CreateFoodItemBulkCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var commandList = new List<CreateFoodItemCommand>();
            foreach (CreateFoodItem.Request request in requests)
            {
                var command = new CreateFoodItemCommand(
                    request.FoodRefId,
                    request.CompartmentId,
                    request.Name,
                    request.Quantity,
                    request.UnitId,
                    request.ExpirationDate.HasValue ? request.ExpirationDate.Value : null,
                    request.Notes);
                commandList.Add(command);
            }
            Result result = await handler.Handle(
                new CreateFoodItemBulkCommand(commandList), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.FoodItems);
    }
}

internal sealed class CreateFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-items", async (
            Request request, 
            ICommandHandler<CreateFoodItemCommand, Guid> handler, 
            CancellationToken cancellationToken) =>
        {

            var command = new CreateFoodItemCommand(
                request.FoodRefId,
                request.CompartmentId,
                request.Name,
                request.Quantity,
                request.UnitId,
                request.ExpirationDate.HasValue ? request.ExpirationDate.Value : null,
                request.Notes);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                guid => Results.CreatedAtRoute(
                    routeName: RouteNames.GetFoodItemById,
                    routeValues: new { id = guid },      
                    value: new { id = guid }),
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.FoodItems);
    }
    internal sealed class Request
    {
        public Guid FoodRefId { get; init; }
        public Guid CompartmentId { get; init; }
        public string? Name { get; init; }
        public decimal Quantity { get; init; }
        public Guid? UnitId { get; init; }
        public DateOnly? ExpirationDate{ get; init; }
        public string? Notes { get; init; }
    }
}
