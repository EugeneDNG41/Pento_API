using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Requests.GetAll;
using Pento.Application.Trades.Requests.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.API.Endpoints.Trades.Get;

internal sealed class GetTradeRequests : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("trades/requests", async (
            Guid? offerId,
            TradeRequestStatus? status,
            bool? isMine,
            GetTradeRequestsSortBy? sortBy,
            SortOrder? sortOrder,
            IQueryHandler<GetTradeRequestsQuery, PagedList<TradeRequestResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10
        ) =>
        {
            var query = new GetTradeRequestsQuery(offerId, status, isMine, sortBy, sortOrder, pageNumber, pageSize);
            Result<PagedList<TradeRequestResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.Trades);
    }
}

