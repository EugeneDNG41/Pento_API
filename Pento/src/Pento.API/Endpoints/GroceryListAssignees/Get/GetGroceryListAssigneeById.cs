using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListAssignees.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListAssignees.Get;

internal sealed class GetGroceryListAssigneeById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("grocery-list-assignees/{id:guid}", async (
            Guid id,
            IQueryHandler<GetGroceryListAssigneeByIdQuery, GroceryListAssigneeResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGroceryListAssigneeByIdQuery(id);

            Result<GroceryListAssigneeResponse> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(
                assignee => Results.Ok(assignee),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryListAssignees);
    }
}
