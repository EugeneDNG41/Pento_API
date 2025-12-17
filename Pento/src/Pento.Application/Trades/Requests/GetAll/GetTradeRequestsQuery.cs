using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Requests.GetAll;

public sealed record GetTradeRequestsQuery(
    Guid? OfferId,
    TradeRequestStatus? Status, 
    bool? IsMine,
    GetTradeRequestsSortBy? SortBy,
    SortOrder? SortOrder,
    int PageNumber, 
    int PageSize) : IQuery<PagedList<TradeRequestResponse>>;
public enum GetTradeRequestsSortBy
{
    CreatedOn,
    TotalItems
}
