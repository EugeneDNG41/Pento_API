using Pento.Domain.Abstractions;
using Pento.Domain.Activities;

namespace Pento.Domain.MilestoneRequirements;

public sealed class MilestoneRequirement
{
    private MilestoneRequirement() { }
    public MilestoneRequirement(Guid id, Guid milestoneId, string activityCode, int quota, int? withinDays)
    {
        Id = id;
        MilestoneId = milestoneId;
        ActivityCode = activityCode;
        Quota = quota;
        WithinDays = withinDays;
    }
    public Guid Id { get; private set; }
    public Guid MilestoneId { get; private set; }
    public string ActivityCode { get; private set; }
    public int Quota { get; private set; }
    public int? WithinDays { get; private set; }
    public static MilestoneRequirement Create(Guid milestoneId, string activityCode, int quota, int? withinDays)
    {
        return new MilestoneRequirement(
            Guid.CreateVersion7(),
            milestoneId,
            activityCode,
            quota,
            withinDays);
    }
    public void UpdateDetails(string? activityCode, int? quota, int? withinDays)
    {
        if (!string.IsNullOrEmpty(activityCode) && ActivityCode != activityCode)
        {
            ActivityCode = activityCode;
        }
        if (quota != null && Quota != quota)
        {
            Quota = quota.Value;
        }
        if (WithinDays != withinDays)
        {
            WithinDays = withinDays;
        }
    }

}
