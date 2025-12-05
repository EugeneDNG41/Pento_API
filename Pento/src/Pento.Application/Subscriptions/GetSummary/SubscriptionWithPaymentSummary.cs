namespace Pento.Application.Subscriptions.GetSummary;

public sealed record SubscriptionWithPaymentSummary
{
    public Guid SubscriptionId { get; init; }
    public string Name { get; init; }
    public List<PaymentByDate> Payments { get; init; } = new List<PaymentByDate>();
}
public sealed record PaymentByDate
{
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public long Amount { get; init; }
    public string Currency { get; init; }
}


