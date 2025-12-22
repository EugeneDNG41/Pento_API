using Pento.Domain.Abstractions;

namespace Pento.Domain.UserActivities;

public sealed class UserActivity : BaseEntity
{
    private UserActivity() { }
    public UserActivity(Guid id, Guid userId, Guid? householdId, string activityCode, DateTime performedOn, Guid? entityId = null)
    {
        Id = id;
        UserId = userId;
        HouseholdId = householdId;
        ActivityCode = activityCode;
        PerformedOn = performedOn;
        EntityId = entityId;
    }
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? HouseholdId { get; private set; }
    public string ActivityCode { get; private set; }
    public DateTime PerformedOn { get; private set; }
    public Guid? EntityId { get; private set; }
    public static UserActivity Create(Guid userId, Guid? householdId, string activityCode, DateTime performedOn, Guid? entityId = null)
    {
        var userActivity = new UserActivity(Guid.CreateVersion7(), userId, householdId, activityCode, performedOn, entityId);
        userActivity.Raise(new UserActivityCreatedDomainEvent(userActivity.Id));
        return userActivity;
    }
}
public sealed class UserActivityCreatedDomainEvent(Guid UserActivityId) : DomainEvent
{
    public Guid UserActivityId { get; } = UserActivityId;
}
