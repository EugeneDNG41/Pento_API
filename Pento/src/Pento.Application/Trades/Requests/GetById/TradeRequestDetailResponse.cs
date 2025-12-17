using Pento.Application.Trades.Requests.GetAll;
using Pento.Application.Trades.Sessions.GetById;

namespace Pento.Application.Trades.Requests.GetById;

public sealed record TradeRequestDetailResponse(TradeRequestResponse TradeRequest, IReadOnlyList<TradeItemResponse> Items);
