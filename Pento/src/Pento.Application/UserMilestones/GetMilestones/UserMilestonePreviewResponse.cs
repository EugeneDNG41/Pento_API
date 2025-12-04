namespace Pento.Application.UserMilestones.GetMilestones;

public sealed record UserMilestonePreviewResponse(
    Guid MilestoneId,
    Uri? Icon,
    string Name,
    DateTime? AchievedOn,
    decimal Progress);
