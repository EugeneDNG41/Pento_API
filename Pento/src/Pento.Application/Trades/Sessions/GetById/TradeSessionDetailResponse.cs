using Pento.Application.Abstractions.Authentication;
using Pento.Application.Trades.Sessions.SendMessage;

namespace Pento.Application.Trades.Sessions.GetById;

public sealed record TradeSessionDetailResponse(TradeSessionResponse TradeSession, IReadOnlyList<TradeSessionMessageResponse> Messages, IReadOnlyList<TradeItemResponse> Items);
