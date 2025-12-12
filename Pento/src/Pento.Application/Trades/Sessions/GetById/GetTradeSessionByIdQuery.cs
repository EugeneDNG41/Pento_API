using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Sessions.GetById;

public sealed record GetTradeSessionByIdQuery(Guid TradeSessionId) : IQuery<TradeSessionDetailResponse>;
