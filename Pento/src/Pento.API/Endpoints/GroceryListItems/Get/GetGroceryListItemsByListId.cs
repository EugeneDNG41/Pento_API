using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListItems.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListItems.Get;

internal sealed class GetGroceryListItemsByListId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("grocery-list-items", async (
            Guid listId,
            IQueryHandler<GetGroceryListItemsByListIdQuery, IReadOnlyList<GroceryListItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGroceryListItemsByListIdQuery(listId);

            Result<IReadOnlyList<GroceryListItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                items => Results.Ok(items),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryListItems);
    }
}
