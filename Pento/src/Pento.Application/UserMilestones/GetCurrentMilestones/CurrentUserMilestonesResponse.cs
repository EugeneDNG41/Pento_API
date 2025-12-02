namespace Pento.Application.UserMilestones.GetCurrentMilestones;

public sealed record CurrentUserMilestonesResponse(
    Guid MilestoneId,
    string Name,
    DateTime? AchievedOn,
    decimal Progress);
