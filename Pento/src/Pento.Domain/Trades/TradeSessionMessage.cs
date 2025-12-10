using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeSessionMessage : Entity
{
    private TradeSessionMessage() { }

    public Guid TradeSessionId { get; private set; }
    public Guid UserId { get; private set; }
    public string MessageText { get; private set; }
    public DateTime SentOn { get; private set; }

    private TradeSessionMessage(
        Guid id,
        Guid tradeSessionId,
        Guid senderUserId,
        string messageText,
        DateTime sentOn
    ) : base(id)
    {
        TradeSessionId = tradeSessionId;
        UserId = senderUserId;
        MessageText = messageText;
        SentOn = sentOn;
    }

    public static TradeSessionMessage Create(Guid sessionId, Guid senderUserId, string message, DateTime sentOn)
    {
        var tradeMessage = new TradeSessionMessage(
            id: Guid.CreateVersion7(),
            tradeSessionId: sessionId,
            senderUserId: senderUserId,
            messageText: message,
            sentOn: sentOn
        );
        tradeMessage.Raise(new TradeSessionMessageCreatedDomainEvent(tradeMessage.Id));
        return tradeMessage;
    }
}
public sealed class TradeSessionMessageCreatedDomainEvent(Guid TradeSessionMessageId) : DomainEvent
{
    public Guid TradeSessionMessageId { get; } = TradeSessionMessageId;
}


