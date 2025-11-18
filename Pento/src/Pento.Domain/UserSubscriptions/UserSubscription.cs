
using Pento.Domain.Shared;

namespace Pento.Domain.UserSubscriptions;

public sealed class UserSubscription
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public SubscriptionStatus Status { get; init; }
    public Guid SubscriptionId { get; init; }
    public DateTime StartDateUtc { get; init; }
    public DateTime? EndDateUtc { get; init; }
}
public sealed class Consumable
{
    public string Name { get; init; }
    public int Usage { get; private set; }
    public int? Quota { get; private set; }
    public bool Usable => Quota is null || Usage < Quota;
    public Period ResetPeriod { get; init; }
    public void ResetUsage()
    {
        Usage = 0;
    }
}
public sealed record NonConsumable
{
    public string Name { get; init; }
    public DateTime? ExpirationDateUtc { get; init; }
}

