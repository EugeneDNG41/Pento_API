using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Trades.Get;

public sealed record GetAllTradePostsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    string? Sort = "newest"
) : IQuery<PagedList<TradePostGroupedResponse>>;
