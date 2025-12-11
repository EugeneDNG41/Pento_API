using Pento.Application.Users.GetAll;

namespace Pento.Application.Trades.Sessions.SendMessage;

public sealed record TradeMessageResponse(
    Guid TradeMessageId, 
    Guid TradeSessionId, 
    string MessageText, 
    DateTime SentOn, 
    BasicUserResponse Sender);
