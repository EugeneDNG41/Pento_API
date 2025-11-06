using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.UserSubscriptions;

public sealed class UserSubscription
{
}
public sealed record UserSubscriptionDetail
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string SubscriptionType { get; init; } = null!;
    public DateTime StartDateUtc { get; init; }
    public DateTime? EndDateUtc { get; init; }
}
