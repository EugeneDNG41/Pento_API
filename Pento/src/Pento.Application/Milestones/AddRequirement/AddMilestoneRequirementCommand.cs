using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.AddRequirement;

public sealed record AddMilestoneRequirementCommand(Guid MilestoneId, string ActivityCode, int Quota, int? WithinDays) : ICommand<Guid>;
