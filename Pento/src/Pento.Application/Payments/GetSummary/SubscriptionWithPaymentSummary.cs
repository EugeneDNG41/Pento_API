using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;

using Pento.Domain.Abstractions;

namespace Pento.Application.Payments.GetSummary;

public enum TimeWindow
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}
public sealed record GetSubscriptionsWithPaymentSummaryQuery(
    Guid[]? SubscriptionId, 
    DateOnly? FromDate, 
    DateOnly? ToDate,
    TimeWindow? TimeWindow,
    bool? IsActive, 
    bool? IsDeleted) : IQuery<IReadOnlyList<SubscriptionWithPaymentSummary>>;
public sealed record GetSubscriptionByIdWithPlanPaymentSummaryQuery(Guid SubscriptionId, DateOnly? FromDate, DateOnly? ToDate) : IQuery<SubscriptionWithPlanPaymentSummary>;
internal sealed class GetSubscriptionsWithPaymentSummaryQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetSubscriptionsWithPaymentSummaryQuery, IReadOnlyList<SubscriptionWithPaymentSummary>>
{
    public async Task<Result<IReadOnlyList<SubscriptionWithPaymentSummary>>> Handle(GetSubscriptionsWithPaymentSummaryQuery query, CancellationToken cancellationToken)
    {
        string dateTrunc = query.TimeWindow switch
        {
            TimeWindow.Daily => "day",
            TimeWindow.Weekly => "week",
            TimeWindow.Monthly => "month",
            TimeWindow.Quarterly => "quarter",
            TimeWindow.Yearly => "year",
            _ => "day"
        };
        string dateInterval = query.TimeWindow switch
        {
            TimeWindow.Daily => "1 day",
            TimeWindow.Weekly => "1 week",
            TimeWindow.Monthly => "1 month",
            TimeWindow.Quarterly => "3 months",
            TimeWindow.Yearly => "1 year",
            _ => "1 day"
        };
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string sql = $"""
            SELECT
                s.id AS SubscriptionId,
                s.name AS Name,
                CASE 
                    WHEN @FromDate IS NULL
                    THEN date_trunc(@dateTrunc, p.paid_at)::date
                    ELSE @FromDate
                END AS FromDate,
                CASE 
                    WHEN @ToDate IS NULL
                    THEN (date_trunc(@dateTrunc, p.paid_at) + interval @dateInterval - interval '1 Day')::date
                    ELSE @ToDate
                END AS ToDate,
                COALESCE(SUM(p.amount_paid), 0) AS Amount,
                p.currency AS Currency
            FROM subscriptions s
            JOIN subscription_plans sp ON sp.subscription_id = s.id
            JOIN payments p ON p.subscription_plan_id = sp.id
            WHERE p.status = 'Paid'
                AND (@IsActive IS NULL OR s.is_active = @IsActive)
                AND (@IsDeleted IS NULL OR s.is_deleted = @IsDeleted)
              AND  (@SubscriptionIds IS NULL OR s.id = ANY(@SubscriptionIds))
              AND (@FromDate IS NULL OR p.date >= @FromDate)
              AND (@ToDate IS NULL OR p.date <= @ToDate)
            GROUP BY s.id, s.name, p.paid_at, p.currency
            ORDER BY p.paid_at;
        """;
        var parameters = new
        {
            dateTrunc,
            dateInterval,
            SubscriptionIds = query.SubscriptionId,
            query.FromDate,
            query.ToDate,
            query.IsActive,
            query.IsDeleted
        };
        var lookup = new Dictionary<Guid, SubscriptionWithPaymentSummary>();
        IEnumerable<SubscriptionWithPaymentSummary> results = await connection.QueryAsync<SubscriptionWithPaymentSummary, PaymentByDate, SubscriptionWithPaymentSummary>(
            sql,
            (subscription, payment) =>
            {
                if (!lookup.TryGetValue(subscription.SubscriptionId, out SubscriptionWithPaymentSummary? sub))
                {
                    sub = subscription;
                    lookup.Add(sub.SubscriptionId, sub);
                }
                if (payment != null)
                {
                    sub.Payments.Add(payment);
                }
                return sub;
            },
            parameters,
            splitOn: "Date"
        );
        return results.ToList();
    }
}


public sealed record SubscriptionWithPaymentSummary
{
    public Guid SubscriptionId { get; init; }
    public string Name { get; init; }
    public List<PaymentByDate> Payments { get; init; } = new List<PaymentByDate>();
}
public sealed record SubscriptionWithPlanPaymentSummary
{
    public Guid SubscriptionId { get; init; }
    public string Name { get; init; }
    public List<SubscriptionPlanPayment> PlanPayments { get; init; } = new List<SubscriptionPlanPayment>();
}
public sealed record SubscriptionPlanPayment
{
    public Guid SubscriptionPlanId { get; init; }
    public List<PaymentByDate> Payments { get; init; } = new List<PaymentByDate>();
}
public sealed record PaymentByDate
{
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public long Amount { get; init; }
    public string Currency { get; init; }

}
