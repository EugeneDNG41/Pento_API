using Pento.Domain.Abstractions;

namespace Pento.Domain.Users.Events;

public sealed class UserHouseholdJoinedDomainEvent(Guid userId, Guid householdId) : DomainEvent
{
    public Guid UserId { get; } = userId;
    public Guid HouseholdId { get; } = householdId;
}
