using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Requests.AdminGetAll;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.AdminGetAll;

public sealed record GetAdminTradeOffersQuery(
    TradeOfferStatus? Status,
    bool? IsDeleted,
    GetTradeOffersSortBy? SortBy,
    SortOrder? SortOrder,
    int PageNumber,
    int PageSize) : IQuery<PagedList<TradeOfferAdminResponse>>;
public enum GetTradeOffersSortBy
{
    CreatedOn,
    TotalItems,
    TotalRequests
}
