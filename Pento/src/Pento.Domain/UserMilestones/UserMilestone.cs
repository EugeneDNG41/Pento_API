using Pento.Domain.Abstractions;

namespace Pento.Domain.UserMilestones;

public sealed class UserMilestone : Entity
{
    private UserMilestone() { }
    public UserMilestone(Guid id, Guid userId, Guid milestoneId, DateTime achievedOn) : base(id)
    {
        UserId = userId;
        MilestoneId = milestoneId;
        AchievedOn = achievedOn;
    }
    public Guid UserId { get; set; }
    public Guid MilestoneId { get; private set; }
    public DateTime AchievedOn { get; private set; }
    public static UserMilestone Create(Guid userId, Guid milestoneId, DateTime achievedOn)
    {
        return new UserMilestone(Guid.NewGuid(), userId, milestoneId, achievedOn);
    }
}
