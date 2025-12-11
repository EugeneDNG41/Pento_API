using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Sessions.RemoveItems;

public sealed record RemoveTradeSessionItemsCommand(Guid TradeSessionId, Guid[] TradeItemIds) : ICommand;
