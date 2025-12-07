using Pento.Domain.Abstractions;

namespace Pento.Domain.Households;

public sealed class HouseholdCreatedDomainEvent(Guid householdId, Guid userId) : DomainEvent
{
    public Guid HouseholdId { get; } = householdId;
    public Guid UserId { get; } = userId;
}
