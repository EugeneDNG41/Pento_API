using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Sessions.Confirm;

public sealed record ConfirmTradeSessionCommand(Guid TradeSessionId) : ICommand;
