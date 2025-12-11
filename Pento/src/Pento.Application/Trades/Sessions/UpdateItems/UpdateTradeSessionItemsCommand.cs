using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.AddItems;

namespace Pento.Application.Trades.Sessions.UpdateItems;

public sealed record UpdateTradeSessionItemsCommand(Guid TradeSessionId, List<UpdateTradeItemDto> Items) : ICommand;
