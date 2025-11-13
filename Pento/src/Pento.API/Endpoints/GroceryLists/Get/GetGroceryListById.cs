using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryLists.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryLists.Get;

internal sealed class GetGroceryListById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("grocery-lists/{id:guid}", async (
            Guid id,
            IQueryHandler<GetGroceryListQuery, GroceryListDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGroceryListQuery(id);
            Result<GroceryListDetailResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryLists)
        .RequireAuthorization();
    }
}
