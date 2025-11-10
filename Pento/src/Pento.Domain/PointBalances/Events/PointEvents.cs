using Pento.Domain.Shared;

namespace Pento.Domain.PointBalances.Events;

public sealed record BalanceCreated //create when first get by id or first point added
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid UserId { get; init; }
}
public sealed record PointAdded
{
    public Guid TaskId { get; init; }
    public ActivityType Category { get; init; }
    public string TaskName { get; init; }
    public int Amount { get; init; }
    public bool CountedTowardsWeeklyCap { get; init; }
    public bool CountedTowardsMonthlyCap { get; init; }
}
public sealed record PointCapReset(Period Period);
