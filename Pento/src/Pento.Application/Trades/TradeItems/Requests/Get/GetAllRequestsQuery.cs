using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.TradeItems.Requests.Get;

public sealed record GetTradeRequestsByOfferQuery(Guid OfferId)
    : IQuery<IReadOnlyList<TradeRequestResponse>>;
