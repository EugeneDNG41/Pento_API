using FluentEmail.Core.Interfaces;
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
        app.MapGet("trades/offers", async (
            int pageNumber,
            int pageSize,
            string? search,
            string? sort,
            IQueryHandler<GetAllTradePostsQuery, PagedList<TradePostGroupedResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetAllTradePostsQuery(
                PageNumber: pageNumber,
                PageSize: pageSize,
                Search: search,
                Sort: sort
            );

            Result<PagedList<TradePostGroupedResponse>> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Trades)
        .WithDescription("sort options: newest | oldest | food | food_desc | quantity");

        app.MapGet("trades/offers/me", async (
                  IQueryHandler<GetMyTradePostsQuery, IReadOnlyList<TradePostResponse>> handler,
                  CancellationToken cancellationToken) =>
        {
            var query = new GetMyTradePostsQuery();

            Result<IReadOnlyList<TradePostResponse>> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(
                list => Results.Ok(list),
                CustomResults.Problem
            );
        })
              .WithTags(Tags.Trades)
              .RequireAuthorization();
    }

}

