using Pento.Domain.Abstractions;

namespace Pento.Domain.Users.Events;

public sealed class UserRegisteredDomainEvent(string identityId) : DomainEvent
{
    public string IdentityId { get; init; } = identityId;
}
