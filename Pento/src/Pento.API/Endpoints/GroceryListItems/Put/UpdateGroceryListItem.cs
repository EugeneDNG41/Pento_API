using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListItems.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListItems.Put;

internal sealed class UpdateGroceryListItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("grocery-list-items/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateGroceryListItemCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateGroceryListItemCommand(
                id,
                request.Quantity,
                request.Notes,
                request.CustomName,
                request.EstimatedPrice,
                request.Priority
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                updatedId => Results.Ok(new { Id = updatedId }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryListItems);
    }

    internal sealed class Request
    {
        public decimal Quantity { get; init; }
        public string? Notes { get; init; }
        public string? CustomName { get; init; }
        public decimal? EstimatedPrice { get; init; }
        public string Priority { get; init; } = "Medium";
    }
}
