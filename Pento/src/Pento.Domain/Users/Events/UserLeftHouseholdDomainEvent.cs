using Pento.Domain.Abstractions;

namespace Pento.Domain.Users.Events;

public sealed class UserLeftHouseholdDomainEvent(Guid userId, Guid householdId) : DomainEvent
{
    public Guid UserId { get; init; } = userId;
    public Guid HouseholdId { get; init; } = householdId;
}
