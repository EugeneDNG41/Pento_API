using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Requests.RemoveItems;

public sealed record RemoveTradeRequestItemsCommand(Guid RequestId, Guid[] TradeItemIds) : ICommand;
