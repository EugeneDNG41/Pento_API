namespace Pento.Application.Subscriptions.GetSummaryById;

public sealed record SubscriptionWithPlanPaymentSummary2
{
    public Guid SubscriptionId { get; init; }
    public string Name { get; init; }
    public List<SubscriptionPlanPayment2> PlanPayments { get; init; } = new List<SubscriptionPlanPayment2>();
}
public sealed record SubscriptionPlanPayment2
{
    public Guid SubscriptionPlanId { get; init; }
    public string Price { get; init; }
    public string Duration { get; init; }
    public List<PaymentByDate2> Payments { get; init; } = new List<PaymentByDate2>();
}
public sealed record PaymentByDate2
{
    public DateOnly Timestamp { get; init; }
    public long Amount { get; init; }
    public string Currency { get; init; }
}
