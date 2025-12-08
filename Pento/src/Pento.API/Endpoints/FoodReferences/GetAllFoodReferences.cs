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
            FoodGroup[]? foodGroup,
            string? search,
            bool? hasImage,
            GetAllFoodReferencesSortBy? sortBy,
            SortOrder? sortOrder,
            IQueryHandler<GetAllFoodReferencesQuery, PagedList<FoodReferenceResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1 ,
            int pageSize = 10
        ) =>
        {
            var query = new GetAllFoodReferencesQuery(
                FoodGroup: foodGroup,
                Search: search,
                HasImage: hasImage,
                sortBy,
                sortOrder,
                Page: pageNumber,
                PageSize: pageSize
            );

            Result<PagedList<FoodReferenceResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodReferences);
    }
}
