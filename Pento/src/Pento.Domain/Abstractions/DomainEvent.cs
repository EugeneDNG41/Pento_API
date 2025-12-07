namespace Pento.Domain.Abstractions;

public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    protected DomainEvent()
    {
        Id = Guid.CreateVersion7();
        Timestamp = DateTime.UtcNow;
    }

    protected DomainEvent(Guid id, DateTime timestamp)
    {
        Id = id;
        Timestamp = timestamp;
    }
}
