using Pento.Domain.Abstractions;


namespace Pento.Domain.UserSubscriptions.Events;

public sealed class UserSubscriptionAdjustedDomainEvent(Guid userSubscriptionId, int durationInDays) : DomainEvent
{
    public Guid UserSubscriptionId { get; } = userSubscriptionId;
    public int DurationInDays { get; } = durationInDays;
}
