using Pento.Domain.Abstractions;

namespace Pento.Domain.Units.Events;

public sealed class UnitCreatedDomainEvent(Guid unitId, string name) : DomainEvent
{
    public Guid UnitId { get; init; } = unitId;
    public string Name { get; init; } = name;
}
