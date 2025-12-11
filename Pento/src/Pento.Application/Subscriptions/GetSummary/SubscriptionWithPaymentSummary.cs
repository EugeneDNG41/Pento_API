namespace Pento.Application.Subscriptions.GetSummary;

public sealed record SubscriptionWithPaymentSummary
{
    public Guid SubscriptionId { get; init; }
    public string Name { get; init; }
    public long TotalPaidAmount => Payments.Sum(p => p.Amount);
    public List<PaymentByDate> Payments { get; init; } = new List<PaymentByDate>();  
}
public sealed record PaymentByDate
{
    public DateOnly Date { get; init; }
    public long Amount { get; init; }
    public string Currency { get; init; }
}


