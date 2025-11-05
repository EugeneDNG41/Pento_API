using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class GetAllFoodReferences : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-references", async (
            [AsParameters] QueryParams queryParams,
            IQueryHandler<GetAllFoodReferencesQuery, PagedFoodReferencesResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllFoodReferencesQuery(
                queryParams.FoodGroup,
                queryParams.Search,
                queryParams.Page,
                queryParams.PageSize
            );

            Result<PagedFoodReferencesResponse> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodReferences);
    }

    internal sealed class QueryParams
    {
        public FoodGroup? FoodGroup { get; init; }
        public string? Search { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
