using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryListItems.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryListItems.Get;

internal sealed class GetGroceryListItemById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("grocery-list-items/{id:guid}", async (
            Guid id,
            IQueryHandler<GetGroceryListItemByIdQuery, GroceryListItemResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGroceryListItemByIdQuery(id);

            Result<GroceryListItemResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                item => Results.Ok(item),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryListItems);
    }
}
