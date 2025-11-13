using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListAssignees.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListAssignees.Delete;

internal sealed class DeleteGroceryListAssignee : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("grocery-list-assignees/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteGroceryListAssigneeCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteGroceryListAssigneeCommand(id);

            Result<Guid> result = await handler.Handle(command, cancellationToken);
            return result.Match(
                  id => Results.Ok(new { Id = id }),
                  CustomResults.Problem
              );
        })
        .WithTags(Tags.GroceryListAssignees);
    }
}
