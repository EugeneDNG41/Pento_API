using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListItems.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListItems.Post;

internal sealed class CreateGroceryListItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("grocery-list-items", async (
            Request request,
            ICommandHandler<CreateGroceryListItemCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateGroceryListItemCommand(
                request.ListId,
                request.FoodRefId,
                request.Quantity,
                request.CustomName,
                request.UnitId,
                request.EstimatedPrice,
                request.Notes,
                request.Priority
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryListItems);
    }

    internal sealed class Request
    {
        public Guid ListId { get; init; }
        public Guid FoodRefId { get; init; }
        public decimal Quantity { get; init; }
        public string? CustomName { get; init; }
        public Guid? UnitId { get; init; }
        public decimal? EstimatedPrice { get; init; }
        public string? Notes { get; init; }
        public string Priority { get; init; } = "Medium";
    }
}
