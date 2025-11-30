using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.Update;

public sealed record UpdateMilestoneCommand(Guid Id, string? Name, string? Description, bool? IsActive) : ICommand;
