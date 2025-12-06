using Pento.Domain.Abstractions;

namespace Pento.Domain.Milestones;

public sealed class MilestoneEnabledOrUpdatedDomainEvent(Guid milestoneId) : DomainEvent
{
    public Guid MilestoneId { get; } = milestoneId;
}
