using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.Delete;

public sealed record DeleteMilestoneCommand(Guid Id) : ICommand;
