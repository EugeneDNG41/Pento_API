using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Offers.RemoveItems;

public sealed record RemoveTradeOfferItemsCommand(Guid OfferId, Guid[] TradeItemIds) : ICommand;
