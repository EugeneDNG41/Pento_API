using Pento.Domain.Abstractions;

namespace Pento.Domain.Households;

public sealed class HouseholdDeletedDomainEvent(Guid householdId) : DomainEvent
{
    public Guid HouseholdId { get; } = householdId;
}
