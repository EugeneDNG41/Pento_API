using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.GetById;

public sealed record GetTradeRequestByIdQuery(Guid TradeRequestId) : IQuery<TradeRequestDetailResponse>;
