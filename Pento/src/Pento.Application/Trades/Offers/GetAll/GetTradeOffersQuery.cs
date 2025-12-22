using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.GetAll;

public sealed record GetTradeOffersQuery(
    bool? IsMine,
    bool? IsMyHousehold,
    TradeOfferStatus? Status,
    PickupOption? Pickup,
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    string? Sort = "newest"
) : IQuery<PagedList<TradeOfferGroupedResponse>>;
