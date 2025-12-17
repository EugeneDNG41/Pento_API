using Pento.Application.Trades.Requests.GetById;
using Pento.Application.Trades.Sessions.GetById;

namespace Pento.Application.Trades.Requests.GetAll;

public sealed record TradeRequestDetailResponse(TradeRequestResponse TradeRequest, IReadOnlyList<TradeItemResponse> Items);
