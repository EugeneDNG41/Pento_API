using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.GetAll;

public sealed record GetTradeSessionsQuery(
    Guid? OfferId, 
    Guid? RequestId, 
    TradeSessionStatus? Status,
    SortOrder? SortOrder,
    int PageNumber,
    int PageSize) : IQuery<PagedList<TradeSessionBasicResponse>>;

        // Note: Total count query is omitted for brevity; in a real implementation, you
