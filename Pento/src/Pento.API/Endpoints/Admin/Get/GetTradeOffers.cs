using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Offers.AdminGetAll;
using Pento.Application.Trades.Requests.AdminGetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetTradeOffers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/trades/offers", async (
            TradeOfferStatus? status,
            bool? isDeleted,
            GetTradeOffersSortBy? sortBy,
            SortOrder? sortOrder,
            IQueryHandler<GetAdminTradeOffersQuery, PagedList<TradeOfferAdminResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10
        ) =>
        {
            var query = new GetAdminTradeOffersQuery(status, isDeleted, sortBy, sortOrder, pageNumber, pageSize);
            Result<PagedList<TradeOfferAdminResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Admin);
    }
}
