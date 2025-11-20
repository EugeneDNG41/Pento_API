using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
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
        app.MapGet("food-items/search", async(
            string? searchText,
            FoodGroup? foodGroup,
            decimal? fromQuantity,
            decimal? toQuantity,
            DateOnly? expirationDateAfter,
            DateOnly? expirationDateBefore,
            FoodItemStatus? status,
            IQueryHandler <SearchFoodItemQuery, PagedList<FoodItemPreview>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<FoodItemPreview>> result = await handler.Handle(
                new SearchFoodItemQuery(
                    searchText,
                    foodGroup,
                    fromQuantity,
                    toQuantity,
                    expirationDateAfter,
                    expirationDateBefore,
                    status,
                    pageNumber,
                    pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization();
    }
}
