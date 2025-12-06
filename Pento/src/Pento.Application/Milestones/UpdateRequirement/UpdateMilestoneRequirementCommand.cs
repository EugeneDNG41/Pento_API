using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Milestones.UpdateRequirement;

public sealed record UpdateMilestoneRequirementCommand(Guid Id, string? ActivityCode, int? Quota, int? WithinDays) : ICommand;
