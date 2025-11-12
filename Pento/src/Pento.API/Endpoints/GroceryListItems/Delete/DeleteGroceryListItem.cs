using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListItems.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListItems.Delete;

internal sealed class DeleteGroceryListItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("grocery-list-items/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteGroceryListItemCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteGroceryListItemCommand(id);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
             deletedId => Results.Ok(new { Id = deletedId }),
             CustomResults.Problem
         );
        })
        .WithTags(Tags.GroceryListItems);
    }
}
