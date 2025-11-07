using Marten.Pagination;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Get;
using Pento.Application.FoodItems.Projections;
using Pento.Application.FoodItems.Search;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.API.Endpoints.FoodItems.Get;

internal sealed class SearchFoodItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("food-items/search", async (           
            Request request,
            IQueryHandler<SearchFoodItemQuery, IPagedList<FoodItemPreview>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<IPagedList<FoodItemPreview>> result = await handler.Handle(
                new SearchFoodItemQuery(
                    request.SearchText,
                    request.FoodGroups,
                    request.FromQuantity,
                    request.ToQuantity,
                    request.ExpirationDateAfter,
                    request.ExpirationDateBefore,
                    pageNumber,
                    pageSize), cancellationToken);
            return result
            .Match(compartment => Results.Ok(compartment), CustomResults.Problem);
        })
        .WithTags(Tags.FoodItems)
        .RequireAuthorization();
    }
    internal sealed class Request
    {
        public string? SearchText { get; init; }
        public List<string> FoodGroups { get; init; } = [];
        public decimal? FromQuantity { get; init; }
        public decimal? ToQuantity { get; init; }
        public DateTime? ExpirationDateAfter { get; init; }
        public DateTime? ExpirationDateBefore { get; init; }
    }
}
