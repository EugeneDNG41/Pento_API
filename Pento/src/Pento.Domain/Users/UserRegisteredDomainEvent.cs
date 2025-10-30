

using Pento.Domain.Abstractions;

namespace Pento.Domain.Users;

public sealed class UserRegisteredDomainEvent(string userId) : DomainEvent
{
    public string IdentityId { get; init; } = userId;
}
