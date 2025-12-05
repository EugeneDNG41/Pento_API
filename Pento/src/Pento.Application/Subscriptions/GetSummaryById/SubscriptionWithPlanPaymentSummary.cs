using Pento.Application.Payments.GetSummaryById;
using Pento.Application.Subscriptions.GetSummary;

namespace Pento.Application.Subscriptions.GetSummaryById;

public sealed record SubscriptionWithPlanPaymentSummary
{
    public Guid SubscriptionId { get; init; }
    public string Name { get; init; }
    public List<SubscriptionPlanPayment> PlanPayments { get; init; } = new List<SubscriptionPlanPayment>();
}
public sealed record SubscriptionPlanPayment
{
    public Guid SubscriptionPlanId { get; init; }
    public string Price { get; init; }
    public string Duration { get; init; }
    public List<PaymentByDate> Payments { get; init; } = new List<PaymentByDate>();
}

