using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.GetById;

namespace Pento.Application.Trades.Sessions.AdminGetById;

public sealed record GetAdminTradeSessionByIdQuery(Guid TradeSessionId) : IQuery<TradeSessionDetailResponse>;
