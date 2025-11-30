using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.Create;

public sealed record CreateMilestoneCommand(string Name, string Description, bool IsActive) : ICommand<Guid>;
