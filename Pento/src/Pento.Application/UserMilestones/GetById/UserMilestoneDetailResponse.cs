namespace Pento.Application.UserMilestones.GetById;

public sealed record UserMilestoneDetailResponse(
    UserMilestoneResponse Milestone,
    IReadOnlyList<UserMilestoneRequirement> Requirements);
public sealed record UserMilestoneResponse
{
    public Guid MilestoneId { get; init; }
    public Uri? Icon { get; init; }
    public string MilestoneName { get; init; }
    public string MilestoneDescription { get; init; }
    public DateTime? AchievedOn { get; init; }
}

public sealed record UserMilestoneRequirement
{
    public int Progress { get; init; }
    public int Quota { get; init; }
    public string TimeFrame { get; init; }
}
