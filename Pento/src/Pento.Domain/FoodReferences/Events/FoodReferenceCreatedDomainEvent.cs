using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodReferences.Events;

public sealed class FoodReferenceCreatedDomainEvent(Guid foodReferenceId) : DomainEvent
{
    public Guid FoodReferenceId { get; init; } = foodReferenceId;

}
