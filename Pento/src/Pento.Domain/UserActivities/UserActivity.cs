namespace Pento.Domain.UserActivities;

public sealed class UserActivity
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
        return new UserActivity(Guid.CreateVersion7(), userId, householdId, activityCode, performedOn, entityId);
    }
}
