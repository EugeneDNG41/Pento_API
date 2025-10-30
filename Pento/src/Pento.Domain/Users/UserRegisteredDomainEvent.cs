

using Pento.Domain.Abstractions;

namespace Pento.Domain.Users;

public sealed class UserRegisteredDomainEvent(string identityId) : DomainEvent
{
    public string IdentityId { get; init; } = identityId;
}
