using Pento.Application.Trades.Requests.AdminGetAll;
using Pento.Application.Trades.Sessions.GetById;

namespace Pento.Application.Trades.Requests.AdminGetById;

public sealed record TradeRequestDetailAdminResponse(TradeRequestAdminResponse TradeRequest, IReadOnlyList<TradeItemResponse> Items);
