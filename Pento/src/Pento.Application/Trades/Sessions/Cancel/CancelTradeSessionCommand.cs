using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Sessions.Cancel;

public sealed record CancelTradeSessionCommand(Guid TradeSessionId) : ICommand;
