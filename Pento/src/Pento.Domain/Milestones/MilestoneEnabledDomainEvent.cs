using Pento.Domain.Abstractions;

namespace Pento.Domain.Milestones;

public sealed class MilestoneEnabledDomainEvent(Guid milestoneId) : DomainEvent
{
    public Guid MilestoneId { get; } = milestoneId;
}
