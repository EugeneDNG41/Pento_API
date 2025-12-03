namespace Pento.Application.UserMilestones.GetMilestones;

public sealed record CurrentUserMilestonesResponse(
    Guid MilestoneId,
    Uri? Icon,
    string Name,
    DateTime? AchievedOn,
    decimal Progress);
