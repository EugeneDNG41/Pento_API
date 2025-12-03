namespace Pento.Application.UserMilestones.GetCurrentMilestones;

public sealed record CurrentUserMilestonesResponse(
    Guid MilestoneId,
    Uri? Icon,
    string Name,
    DateTime? AchievedOn,
    decimal Progress);
