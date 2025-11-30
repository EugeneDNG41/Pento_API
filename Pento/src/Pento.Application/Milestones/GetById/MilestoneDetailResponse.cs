namespace Pento.Application.Milestones.GetById;

public sealed record MilestoneDetailResponse(MilestoneResponse Milestone, IReadOnlyList<MilestoneRequirementResponse> Requirements);
public sealed record MilestoneResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public bool IsActive { get; init; }
}
public sealed record MilestoneRequirementResponse
{
    public string Id { get; init; }
    public string Activity { get; init; }
    public int Quota { get; init; }
    public string TimeFrame { get; init; }
}
