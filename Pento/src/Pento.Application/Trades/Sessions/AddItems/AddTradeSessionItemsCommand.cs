using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Sessions.AddItems;

public sealed record AddTradeSessionItemsCommand(Guid TradeSessionId, List<AddTradeItemDto> Items) : ICommand<IReadOnlyList<TradeItemResponse>>;
