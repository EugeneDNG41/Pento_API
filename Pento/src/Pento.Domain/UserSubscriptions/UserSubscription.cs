
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
    public int? RemainingDaysAfterPause { get; private set; }
    public DateOnly? CancelledDate { get; private set; }
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
        Status = SubscriptionStatus.Active; //business rule: renewing a subscription always sets it to Active
        if (newEndDate != null && RemainingDaysAfterPause != null)
        {
            EndDate = newEndDate.Value.AddDays(RemainingDaysAfterPause.Value);
            RemainingDaysAfterPause = null;
        }
        else
        {
            EndDate = newEndDate;
        }
    }
    public void Pause(DateOnly pausedDate)
    {
        Status = SubscriptionStatus.Paused;
        PausedDate = pausedDate;
        EndDate = null;
        RemainingDaysAfterPause = EndDate.HasValue ? EndDate.Value.DayNumber - pausedDate.DayNumber : null;
    }
    public void Resume(DateOnly resumedDate)
    {
        Status = SubscriptionStatus.Active;
        EndDate = RemainingDaysAfterPause.HasValue? resumedDate.AddDays(RemainingDaysAfterPause.Value) : null;
        PausedDate = null;
        RemainingDaysAfterPause = null;
    }
    public void Cancel(DateOnly cancelledDate, string? cancellationReason)
    {
        Status = SubscriptionStatus.Cancelled;
        CancelledDate = cancelledDate;
        CancellationReason = cancellationReason;
        EndDate = cancelledDate;
    }
}


