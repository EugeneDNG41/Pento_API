using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Get;

public sealed record GetAllTradePostsQuery(
    bool? IsMine,
    bool? IsMyHousehold,
    TradeOfferStatus? Status,
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    string? Sort = "newest"
) : IQuery<PagedList<TradePostGroupedResponse>>;
