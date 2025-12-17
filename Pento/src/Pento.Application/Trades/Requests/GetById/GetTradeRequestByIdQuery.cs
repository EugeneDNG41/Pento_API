using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Requests.GetAll;

namespace Pento.Application.Trades.Requests.GetById;

public sealed record GetTradeRequestByIdQuery(Guid TradeRequestId) : IQuery<TradeRequestDetailResponse>;
