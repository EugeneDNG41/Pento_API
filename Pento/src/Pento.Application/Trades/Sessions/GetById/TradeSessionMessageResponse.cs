using Pento.Application.Users.GetAll;

namespace Pento.Application.Trades.Sessions.GetById;

public sealed record TradeSessionMessageResponse
{
    public Guid TradeSessionMessageId { get; init; }
    public BasicUserResponse User { get; init; }
    public string MessageText { get; init; }
    public DateTime SentOn { get; init; }
}
