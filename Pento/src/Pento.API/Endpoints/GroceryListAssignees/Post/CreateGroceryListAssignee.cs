using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListAssignees.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListAssignees.Post;

internal sealed class CreateGroceryListAssignee : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("grocery-list-assignees", async (
            Request request,
            ICommandHandler<CreateGroceryListAssigneeCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateGroceryListAssigneeCommand(
                request.GroceryListId,
                request.HouseholdMemberId
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryListAssignees);
    }

    internal sealed class Request
    {
        public Guid GroceryListId { get; init; }
        public Guid HouseholdMemberId { get; init; }
    }
}
