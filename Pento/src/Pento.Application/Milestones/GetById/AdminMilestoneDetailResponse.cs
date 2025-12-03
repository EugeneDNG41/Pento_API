namespace Pento.Application.Milestones.GetById;

public sealed record AdminMilestoneDetailResponse(AdminMilestoneResponse Milestone, IReadOnlyList<AdminMilestoneRequirementResponse> Requirements); //turn into admin response

public sealed record AdminMilestoneResponse
{
    public Guid Id { get; init; }
    public Uri? Icon { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public bool IsActive { get; init; }
    public int EarnedCount { get; init; }
    public bool IsDeleted { get; init; }
}
public sealed record AdminMilestoneRequirementResponse
{
    public string Id { get; init; }
    public string Activity { get; init; }
    public int Quota { get; init; }
    public string TimeFrame { get; init; }
}
