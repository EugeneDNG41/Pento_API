using Pento.Domain.Abstractions;

namespace Pento.Domain.Users.Events;

public sealed class UserLeftHouseholdDomainEvent(Guid householdId) : DomainEvent
{
    public Guid HouseholdId { get; init; } = householdId;
}
