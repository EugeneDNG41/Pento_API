using Pento.Domain.Abstractions;

namespace Pento.Domain.UserMilestones;

public sealed class UserMilestoneCreatedDomainEvent(Guid userMilestoneId) : DomainEvent
{
    public Guid UserMilestoneId { get; } = userMilestoneId;
}
