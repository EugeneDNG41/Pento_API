using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.FoodItemLogs.Search;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemLogs;

namespace Pento.API.Endpoints.FoodItemLogs;

internal sealed class SearchFoodItemLog : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-item-logs", async (
            string? searchText,
            DateTime? fromDate,
            DateTime? toDate,
            FoodItemLogAction? logAction,
            IQueryHandler<SearchFoodItemLogQuery, PagedList<FoodItemLogPreview>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<FoodItemLogPreview>> result = await handler.Handle(new SearchFoodItemLogQuery(
                searchText,
                fromDate?.ToUniversalTime(),
                toDate?.ToUniversalTime(),
                logAction,
                pageNumber,
                pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodItemLogs)
        .RequireAuthorization()
        .WithDescription("GetAll food item logs");
    }
}
