
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.UserSubscriptions;

public sealed class UserSubscription : Entity
{
    public Guid UserId { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public SubscriptionStatus Status { get; private set; }   
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public DateOnly? PausedDate { get; private set; }
    public DateOnly? ResumedDate { get; private set; }
    public DateOnly? CancelledDateUtc { get; private set; }
    public string? CancellationReason { get; private set; }
}


