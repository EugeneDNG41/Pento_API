using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Offers.AdminGetById;

public sealed record GetAdminTradeOfferByIdQuery(Guid TradeOfferId) : IQuery<TradeOfferDetailAdminResponse>;
