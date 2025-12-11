using System.Numerics;

namespace Pento.Application.Subscriptions.GetSummaryById;

public sealed record SubscriptionWithPlanPaymentSummary2
{
    public Guid SubscriptionId { get; init; }
    public string Name { get; init; }
    public long TotalPaidAmount
    {
        get
        {
            long total = 0;
            foreach (SubscriptionPlanPayment2 planPayment in PlanPayments)
            {
                foreach (PaymentByDate2 payment in planPayment.Payments)
                {
                    total += payment.Amount;
                }
            }
            return total;
        }
    }
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
    public DateOnly Date { get; init; }
    public long Amount { get; init; }
    public string Currency { get; init; }
}
