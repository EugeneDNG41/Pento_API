using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Compartments.Get;
using Pento.Application.FoodItems.Search;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;

namespace Pento.API.Endpoints.FoodItems.Get;

internal sealed class SearchFoodItemGet : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-items", async(
            Guid? foodReferenceId,
            string? searchText,
            FoodGroup[]? foodGroup,
            FoodItemStatus[]? status,
            decimal? fromQuantity,
            decimal? toQuantity,
            DateOnly? expirationDateAfter,
            DateOnly? expirationDateBefore,
            IQueryHandler <SearchFoodItemQuery, PagedList<FoodItemPreview>> handler,
            CancellationToken cancellationToken,
            FoodItemPreviewSortBy sortBy = FoodItemPreviewSortBy.Default,
            SortOrder sortOrder = SortOrder.ASC,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<FoodItemPreview>> result = await handler.Handle(
                new SearchFoodItemQuery(
                    foodReferenceId,
                    searchText,
                    foodGroup,
                    status,
                    fromQuantity,
                    toQuantity,
                    expirationDateAfter,
                    expirationDateBefore,
                    sortBy,
                    sortOrder,
                    pageNumber,
                    pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization();
    }
}
