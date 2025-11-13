using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListAssignees.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListAssignees.Get;

internal sealed class GetGroceryListAssigneesByListId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("grocery-list-assignees", async (
            Guid groceryListId,
            IQueryHandler<GetGroceryListAssigneesByListIdQuery, IReadOnlyList<GroceryListAssigneeResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGroceryListAssigneesByListIdQuery(groceryListId);

            Result<IReadOnlyList<GroceryListAssigneeResponse>> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(
                assignees => Results.Ok(assignees),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryListAssignees);
    }
}
