using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryLists.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryLists.Delete;

internal sealed class DeleteGroceryList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("grocery-lists/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteGroceryListCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteGroceryListCommand(id);
            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                deletedId => Results.Ok(new { Id = deletedId }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryLists)
        .RequireAuthorization();
    }
}
