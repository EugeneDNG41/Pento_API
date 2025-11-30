using Pento.Domain.Abstractions;

namespace Pento.Domain.Milestones;

public static class MilestoneErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Milestone.NotFound",
        "Milestone not found."
    );
    public static readonly Error UserMilestoneNotFound = Error.NotFound(
        "UserMilestone.NotFound",
        "User milestone not found."
    );
    public static readonly Error RequirementNotFound = Error.NotFound(
        "MilestoneRequirement.NotFound",
        "Milestone requirement not found."
    );
    public static readonly Error NameTaken = Error.Conflict(
        "Milestone.NameTaken",
        "Milestone name is already taken." //business rule
    );
    public static readonly Error MilestoneInUse = Error.Conflict(
        "Milestone.InUse",
        "Milestone cannot be deleted as it is associated with active users." //business rule
    );
    public static readonly Error DuplicateRequirement = Error.Conflict(
        "MilestoneRequirement.Duplicate",
        "A requirement for this activity already exists for the milestone." //business rule
    );
}
