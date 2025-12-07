using Pento.Domain.Abstractions;

namespace Pento.Domain.UserEntitlements.Events;

public sealed class EntitlementResetDomainEvent(Guid entitlementId) : DomainEvent
{
    public Guid UserEntitlementId { get; } = entitlementId;
}
