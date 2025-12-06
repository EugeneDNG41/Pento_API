namespace Pento.Infrastructure.Utility.Outbox;

public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Content = content;
        Type = type;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }

    public string Type { get; init; }

    public string Content { get; init; }

    public DateTime? ProcessedOnUtc { get; init; }

    public string? Error { get; init; }
    public static OutboxMessage Create(DateTime occurredOnUtc, string type, string content)
    {
        return new OutboxMessage(
            id: Guid.CreateVersion7(),
            occurredOnUtc: occurredOnUtc,
            type: type,
            content: content);
    }
}
