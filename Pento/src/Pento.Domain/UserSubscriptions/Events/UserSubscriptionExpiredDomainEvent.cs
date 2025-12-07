using Pento.Domain.Abstractions;


namespace Pento.Domain.UserSubscriptions.Events;

public sealed class UserSubscriptionExpiredDomainEvent(Guid userSubscriptionId) : DomainEvent
{
    public Guid UserSubscriptionId { get; } = userSubscriptionId;
}
