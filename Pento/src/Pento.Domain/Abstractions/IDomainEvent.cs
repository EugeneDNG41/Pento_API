namespace Pento.Domain.Abstractions;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime Timestamp { get; }
}
