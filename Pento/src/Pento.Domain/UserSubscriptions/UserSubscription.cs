
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.UserSubscriptions;

public sealed class UserSubscription : Entity
{
    private UserSubscription() { }
    public UserSubscription(
        Guid id,
        Guid userId,
        Guid subscriptionId,
        SubscriptionStatus status,
        DateOnly startDate,
        DateOnly? endDate) : base(id)
    {
        UserId = userId;
        SubscriptionId = subscriptionId;
        Status = status;
        StartDate = startDate;
        EndDate = endDate;
    }
    public Guid UserId { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public SubscriptionStatus Status { get; private set; }   
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public DateOnly? PausedDate { get; private set; }
    public DateOnly? ResumedDate { get; private set; }
    public DateOnly? CancelledDateUtc { get; private set; }
    public string? CancellationReason { get; private set; }
    public static UserSubscription Create(
        Guid userId,
        Guid subscriptionId,
        SubscriptionStatus status,
        DateOnly startDate,
        DateOnly? endDate)
        => new(Guid.CreateVersion7(), userId, subscriptionId, status, startDate, endDate);
    public void Renew(DateOnly? newEndDate)
    {
        Status = SubscriptionStatus.Active;
        if (newEndDate is not null)
        {
            EndDate = newEndDate;
        }
    }
}


