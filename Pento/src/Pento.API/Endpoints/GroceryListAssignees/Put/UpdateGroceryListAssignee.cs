using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListAssignees.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListAssignees.Put;

internal sealed class UpdateGroceryListAssignee : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("grocery-list-assignees/{id:guid}", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateGroceryListAssigneeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateGroceryListAssigneeCommand(
                id,
                request.IsCompleted
            );

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryListAssignees);
    }

    internal sealed class Request
    {
        public bool IsCompleted { get; init; }
    }
}
