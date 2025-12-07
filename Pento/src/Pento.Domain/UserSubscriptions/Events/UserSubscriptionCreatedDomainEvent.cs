using Pento.Domain.Abstractions;


namespace Pento.Domain.UserSubscriptions.Events;

public sealed class UserSubscriptionCreatedDomainEvent(Guid userSubscriptionId) : DomainEvent
{
    public Guid UserSubscriptionId { get; } = userSubscriptionId;
}
