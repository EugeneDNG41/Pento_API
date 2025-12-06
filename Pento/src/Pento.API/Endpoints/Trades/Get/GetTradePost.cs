using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Trades.Get;

internal sealed class GetTradePost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trade-posts", async (
            int pageNumber,
            int pageSize,
            string? search,
            string? sort,
            IQueryHandler<GetAllTradePostsQuery, PagedList<TradePostResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetAllTradePostsQuery(
                PageNumber: pageNumber,
                PageSize: pageSize,
                Search: search,
                Sort: sort
            );

            Result<PagedList<TradePostResponse>> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .WithDescription("sort options: newest | oldest | food | food_desc | quantity");
    }
}
