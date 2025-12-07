using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.TradeItem.Requests.Get;

public sealed record GetTradeRequestsByOfferQuery(Guid OfferId)
    : IQuery<IReadOnlyList<TradeRequestResponse>>;
