using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Subscriptions;

public sealed class SubscriptionPlan : Entity
{
    private SubscriptionPlan() { }
    public SubscriptionPlan(Guid id, Guid subscriptionId, long amount, Currency currency, int? duration) : base(id)
    {
        SubscriptionId = subscriptionId;
        Amount = amount;
        Currency = currency;
        DurationInDays = duration;
    }
    public Guid SubscriptionId { get; private set; }
    public long Amount { get; private set; }
    public Currency Currency { get; private set; }
    public int? DurationInDays { get; private set; }
    public static SubscriptionPlan Create(Guid subscriptionId, long amount, Currency currency, int? duration)
    {
        return new SubscriptionPlan(Guid.NewGuid(), subscriptionId, amount, currency, duration);
    }
    public void UpdateDetails(long? amount, Currency? currency, int? duration)
    {
        if (amount.HasValue && Amount != amount)
        {
            Amount = amount.Value;
        }
        if (currency.HasValue && Currency != currency)
        {
            Currency = currency.Value;
        }
        if (DurationInDays != duration)
        {
            DurationInDays = duration;
        }
    }
}
