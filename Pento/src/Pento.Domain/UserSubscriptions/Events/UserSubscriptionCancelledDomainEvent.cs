using Pento.Domain.Abstractions;


namespace Pento.Domain.UserSubscriptions.Events;

public sealed class UserSubscriptionCancelledDomainEvent(Guid userSubscriptionId) : DomainEvent
{
    public Guid UserSubscriptionId { get; } = userSubscriptionId;
}
