using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.FoodReferences.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class GetAllFoodReferences : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-references", async (
            FoodGroup? foodGroup,
            string? search,
            int pageNumber,
            int pageSize,
            IQueryHandler<GetAllFoodReferencesQuery, PagedList<FoodReferenceResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetAllFoodReferencesQuery(
                FoodGroup: foodGroup,
                Search: search,
                Page: pageNumber,
                PageSize: pageSize
            );

            Result<PagedList<FoodReferenceResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodReferences);
    }
}
