using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodReferences.Events;

public sealed class FoodReferenceUpdatedDomainEvent(Guid foodReferenceId) : DomainEvent
{
    public Guid FoodReferenceId { get; init; } = foodReferenceId;

}
