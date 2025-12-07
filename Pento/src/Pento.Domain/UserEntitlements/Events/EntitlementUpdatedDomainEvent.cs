using Pento.Domain.Abstractions;

namespace Pento.Domain.UserEntitlements.Events;

public sealed class EntitlementUpdatedDomainEvent(Guid entitlementId) : DomainEvent
{
    public Guid UserEntitlementId { get; } = entitlementId;
}
