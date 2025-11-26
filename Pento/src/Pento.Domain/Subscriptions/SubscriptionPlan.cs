using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Subscriptions;

public sealed class SubscriptionPlan : Entity
{
    private SubscriptionPlan() { }
    public SubscriptionPlan(Guid id, Guid subscriptionId, Money price, Duration? duration) : base(id)
    {
        SubscriptionId = subscriptionId;
        Price = price;
        Duration = duration;
    }
    public Guid SubscriptionId { get; private set; }
    public Money Price { get; private set; }
    public Duration? Duration { get; private set; }
    public static SubscriptionPlan Create(Guid subscriptionId, Money price, Duration? duration)
        => new(Guid.CreateVersion7(), subscriptionId, price, duration);
    public void UpdateDetails(Money? price, Duration? duration)
    {
        if (price is not null)
        {
            Price = price;
        }
        if (duration is not null)
        {
            Duration = duration;
        }
    }
}
