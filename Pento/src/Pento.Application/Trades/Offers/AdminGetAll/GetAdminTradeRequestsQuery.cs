using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Requests.AdminGetAll;
using Pento.Application.Trades.Requests.GetAll;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.AdminGetAll;

public sealed record GetAdminTradeRequestsQuery(
    Guid? OfferId,
    TradeRequestStatus? Status,
    bool? IsDeleted,
    GetTradeRequestsSortBy? SortBy,
    SortOrder? SortOrder,
    int PageNumber,
    int PageSize) : IQuery<PagedList<TradeRequestAdminResponse>>;

