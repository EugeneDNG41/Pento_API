using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Sessions.SendMessage;

public sealed record SendTradeMessageCommand(Guid TradeSessionId, string Message) : ICommand<TradeMessageResponse>;
