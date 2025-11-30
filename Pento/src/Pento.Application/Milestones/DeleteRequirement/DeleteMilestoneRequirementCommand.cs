using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.DeleteRequirement;

public sealed record DeleteMilestoneRequirementCommand(Guid Id) : ICommand;
