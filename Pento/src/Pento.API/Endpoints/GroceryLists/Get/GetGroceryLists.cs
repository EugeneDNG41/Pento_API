using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GroceryLists.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.GroceryLists.Get;

internal sealed class GetGroceryLists : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("grocery-lists", async (
            Guid householdId,
            IQueryHandler<GetGroceryListsByHouseholdIdQuery, IReadOnlyList<GroceryListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetGroceryListsByHouseholdIdQuery(householdId);

            Result<IReadOnlyList<GroceryListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GroceryLists);
    }
}
