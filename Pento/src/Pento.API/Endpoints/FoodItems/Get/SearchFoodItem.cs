using MailKit.Search;
using Marten.Pagination;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Get;
using Pento.Application.FoodItems.Projections;
using Pento.Application.FoodItems.Search;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.API.Endpoints.FoodItems.Get;

internal sealed class SearchFoodItemGet : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-items/search", async(
            string? searchText,
            string? foodGroups,
            decimal? fromQuantity,
            decimal? toQuantity,
            DateTime? expirationDateAfter,
            DateTime? expirationDateBefore,
            IQueryHandler <SearchFoodItemQuery, IPagedList<FoodItemPreview>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<IPagedList<FoodItemPreview>> result = await handler.Handle(
                new SearchFoodItemQuery(
                    searchText,
                    foodGroups,
                    fromQuantity,
                    toQuantity,
                    expirationDateAfter,
                    expirationDateBefore,
                    pageNumber,
                    pageSize), cancellationToken);
            return result
            .Match(compartment => Results.Ok(compartment), CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization();
    }
}
