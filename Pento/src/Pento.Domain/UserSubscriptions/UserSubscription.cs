using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Shared;

namespace Pento.Domain.UserSubscriptions;

public sealed class UserSubscription
{
}
public sealed record UserSubscriptionInstance
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public SubscriptionStatus Status { get; init; }
    public DateTimeOffset StartDateUtc { get; init; }


    public DateTimeOffset? EndDateUtc { get; init; }
    public List<Consumable> Consumables { get; init; }
    public List<NonConsumable> NonConsumables { get; init; }

}
public enum SubscriptionStatus 
{
    Active,
    Paused,
    Canceled,
    Expired
}
public sealed class Consumable
{
    public Guid Id { get; init; } // feature id
    public string Name { get; init; }
    public int Usage { get; private set; }
    public int Quota { get; private set; }
    public bool Usable => Usage < Quota;
    public Interval ResetInterval { get; init; }
    public void ResetUsage()
    {
        Usage = 0;
    }
}
public sealed record NonConsumable
{
    public Guid Id { get; init; } // feature id
    public string Name { get; init; }
    public DateTime? ExpirationDateUtc { get; init; }
}
