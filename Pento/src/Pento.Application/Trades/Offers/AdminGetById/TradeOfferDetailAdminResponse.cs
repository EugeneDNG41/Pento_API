using Pento.Application.Trades.Requests.AdminGetAll;
using Pento.Application.Trades.Requests.AdminGetById;
using Pento.Application.Trades.Sessions.GetById;

namespace Pento.Application.Trades.Offers.AdminGetById;

public sealed record TradeOfferDetailAdminResponse(TradeOfferAdminResponse TradeOffer, IReadOnlyList<TradeItemResponse> Items);
