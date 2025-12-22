using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Offers.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.API.Endpoints.Trades.Get;

internal sealed class GetTradePost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trades/offers", async (
            bool? isMine,
            bool? isMyHousehold,
            TradeOfferStatus? status,
            PickupOption? pickup,
            string? search,
            string? sort,
            IQueryHandler<GetTradeOffersQuery, PagedList<TradeOfferGroupedResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10
        ) =>
        {
            var query = new GetTradeOffersQuery(
                isMine, isMyHousehold, status, pickup,
                PageNumber: pageNumber,
                PageSize: pageSize,
                Search: search,
                Sort: sort
            );

            Result<PagedList<TradeOfferGroupedResponse>> result =
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

