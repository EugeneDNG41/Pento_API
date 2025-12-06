using Pento.Domain.Abstractions;

namespace Pento.Domain.Users.Events;

public sealed class UserCreatedDomainEvent(Guid userId) : DomainEvent
{
    public Guid UserId { get; } = userId;
}
