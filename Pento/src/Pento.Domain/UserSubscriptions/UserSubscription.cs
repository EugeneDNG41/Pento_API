
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
        DateOnly startDate,
        DateOnly? endDate)
    {  
        var userSubscription =  new UserSubscription(Guid.CreateVersion7(), userId, subscriptionId, SubscriptionStatus.Active, startDate, endDate);
        userSubscription.Raise(new UserSubscriptionActivatedDomainEvent(userSubscription.Id));
        return userSubscription;
    }
       
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
        Raise(new UserSubscriptionActivatedDomainEvent(Id));
    }
    public void Pause(DateOnly pausedDate)
    {
        Status = SubscriptionStatus.Paused;
        PausedDate = pausedDate;
        EndDate = null;
        RemainingDaysAfterPause = EndDate.HasValue ? EndDate.Value.DayNumber - pausedDate.DayNumber : null;
        Raise(new UserSubscriptionDeactivatedDomainEvent(Id));
    }
    public void Resume(DateOnly resumedDate)
    {
        Status = SubscriptionStatus.Active;
        EndDate = RemainingDaysAfterPause.HasValue? resumedDate.AddDays(RemainingDaysAfterPause.Value) : null;
        PausedDate = null;
        RemainingDaysAfterPause = null;
        Raise(new UserSubscriptionActivatedDomainEvent(Id));
    }
    public void Cancel(DateOnly cancelledDate, string? cancellationReason)
    {
        Status = SubscriptionStatus.Cancelled;
        CancelledDate = cancelledDate;
        CancellationReason = cancellationReason;
        EndDate = cancelledDate;
        Raise(new UserSubscriptionDeactivatedDomainEvent(Id));
    }
    public void Expire()
    {
        Status = SubscriptionStatus.Expired;
        Raise(new UserSubscriptionDeactivatedDomainEvent(Id));
    }
}
public sealed class UserSubscriptionActivatedDomainEvent(Guid userSubscriptionId) : DomainEvent
{
    public Guid UserSubscriptionId { get; } = userSubscriptionId;
}

public sealed class UserSubscriptionDeactivatedDomainEvent(Guid userSubscriptionId) : DomainEvent
{
    public Guid UserSubscriptionId { get; } = userSubscriptionId;
}
