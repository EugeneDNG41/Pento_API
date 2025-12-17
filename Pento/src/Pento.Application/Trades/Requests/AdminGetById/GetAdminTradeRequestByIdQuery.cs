using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.AdminGetById;

public sealed record GetAdminTradeRequestByIdQuery(Guid TradeRequestId) : IQuery<TradeRequestDetailAdminResponse>;
