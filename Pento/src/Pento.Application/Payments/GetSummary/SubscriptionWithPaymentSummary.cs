using System.Data.Common;
using System.Linq;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;

using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;
using static Pento.Application.Payments.GetSummary.GetSubscriptionByIdWithPlanPaymentSummaryQueryHandler;

namespace Pento.Application.Payments.GetSummary;

public enum TimeWindow
{
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}
public sealed record GetSubscriptionsWithPaymentSummaryQuery(
    DateOnly? FromDate, 
    DateOnly? ToDate,
    TimeWindow? TimeWindow,
    bool? IsActive, 
    bool? IsDeleted) : IQuery<IReadOnlyList<SubscriptionWithPaymentSummary>>;
internal sealed class GetSubscriptionsWithPaymentSummaryQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetSubscriptionsWithPaymentSummaryQuery, IReadOnlyList<SubscriptionWithPaymentSummary>>
{
    public async Task<Result<IReadOnlyList<SubscriptionWithPaymentSummary>>> Handle(GetSubscriptionsWithPaymentSummaryQuery query, CancellationToken cancellationToken)
    {
        string dateTrunc = query.TimeWindow switch
        {
            TimeWindow.Weekly => "week",
            TimeWindow.Monthly => "month",
            TimeWindow.Quarterly => "quarter",
            TimeWindow.Yearly => "year",
            _ => "day"
        };
        string dateInterval = query.TimeWindow switch
        {
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
                COALESCE(@FromDate::date, date_trunc(@dateTrunc, p.paid_at)::date) AS FromDate,
                COALESCE(@ToDate::date, (date_trunc(@dateTrunc, p.paid_at) + (@dateInterval)::interval - interval '1 day')::date
                ) AS ToDate,
                COALESCE(SUM(p.amount_paid), 0) AS Amount,
                p.currency AS Currency
            FROM subscriptions s
            JOIN subscription_plans sp ON sp.subscription_id = s.id
            JOIN payments p ON p.subscription_plan_id = sp.id
            WHERE p.status = 'Paid'
                AND (@IsActive IS NULL OR s.is_active = @IsActive)
                AND (@IsDeleted IS NULL OR s.is_deleted = @IsDeleted)
              AND (@FromDate IS NULL OR p.paid_at >= @FromDate)
              AND (@ToDate IS NULL OR p.paid_at <= @ToDate)
            GROUP BY s.id, s.name, 
                COALESCE(@FromDate::date, date_trunc(@dateTrunc, p.paid_at)::date), 
                COALESCE(@ToDate::date, (date_trunc(@dateTrunc, p.paid_at) + (@dateInterval)::interval - interval '1 day')::date),
                p.currency
            ORDER BY COALESCE(@FromDate::date, date_trunc(@dateTrunc, p.paid_at)::date), 
                COALESCE(@ToDate::date, (date_trunc(@dateTrunc, p.paid_at) + (@dateInterval)::interval - interval '1 day')::date);
        """;
        var parameters = new
        {
            dateTrunc,
            dateInterval,
            query.FromDate,
            query.ToDate,
            query.IsActive,
            query.IsDeleted
        };
        var command = new CommandDefinition(
            commandText: sql,
            parameters: parameters,
            cancellationToken: cancellationToken
        );
        var lookup = new Dictionary<Guid, SubscriptionWithPaymentSummary>();
        await connection.QueryAsync<SubscriptionWithPaymentSummary, PaymentByDate, SubscriptionWithPaymentSummary>(
            command: command,
            (subscription, payment) =>
            {
                if (lookup.TryGetValue(subscription.SubscriptionId, out SubscriptionWithPaymentSummary existingSub))
                {
                    subscription = existingSub;
                }
                else
                {
                    lookup.Add(subscription.SubscriptionId, subscription);
                }
                subscription.Payments.Add(payment);
                return subscription;
            },
            splitOn: "FromDate"
        );
        return lookup.Values.ToList();
    }
}

public sealed record GetSubscriptionWithPlanPaymentSummaryByIdQuery(
    Guid SubscriptionId,
    DateOnly? FromDate, 
    DateOnly? ToDate,
    TimeWindow? TimeWindow) : IQuery<SubscriptionWithPlanPaymentSummary>;
internal sealed class GetSubscriptionByIdWithPlanPaymentSummaryQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetSubscriptionWithPlanPaymentSummaryByIdQuery, SubscriptionWithPlanPaymentSummary>
{
    public async Task<Result<SubscriptionWithPlanPaymentSummary>> Handle(GetSubscriptionWithPlanPaymentSummaryByIdQuery query, CancellationToken cancellationToken)
    {
        string dateTrunc = query.TimeWindow switch
        {
            TimeWindow.Weekly => "week",
            TimeWindow.Monthly => "month",
            TimeWindow.Quarterly => "quarter",
            TimeWindow.Yearly => "year",
            _ => "day"
        };
        string dateInterval = query.TimeWindow switch
        {
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
                sp.id AS SubscriptionPlanId,
                CONCAT(sp.amount::text, ' ', sp.currency) AS price,
                         CASE
                             WHEN duration_in_days IS NULL THEN 'Lifetime'
                             ELSE CONCAT(sp.duration_in_days::text, ' ', 'day',
                                 CASE 
                                     WHEN COALESCE(sp.duration_in_days,0) = 1 THEN '' ELSE 's' 
                                 END)
                         END
                         AS duration,
                COALESCE(@FromDate::date, date_trunc(@dateTrunc, p.paid_at)::date) AS FromDate,
                COALESCE(@ToDate::date, (date_trunc(@dateTrunc, p.paid_at) + (@dateInterval)::interval - interval '1 day')::date
                ) AS ToDate,
                COALESCE(SUM(p.amount_paid), 0) AS Amount,
                p.currency AS Currency
            FROM subscriptions s
            JOIN subscription_plans sp ON sp.subscription_id = s.id
            JOIN payments p ON p.subscription_plan_id = sp.id
            WHERE p.status = 'Paid'
                AND s.id = @SubscriptionId
              AND (@FromDate IS NULL OR p.paid_at >= @FromDate)
              AND (@ToDate IS NULL OR p.paid_at <= @ToDate)
            GROUP BY s.id, s.name, sp.id,
                COALESCE(@FromDate::date, date_trunc(@dateTrunc, p.paid_at)::date), 
                COALESCE(@ToDate::date, (date_trunc(@dateTrunc, p.paid_at) + (@dateInterval)::interval - interval '1 day')::date),
                p.currency
            ORDER BY COALESCE(@FromDate::date, date_trunc(@dateTrunc, p.paid_at)::date), 
                COALESCE(@ToDate::date, (date_trunc(@dateTrunc, p.paid_at) + (@dateInterval)::interval - interval '1 day')::date);
         """;
        var parameters = new
        {
            dateTrunc,
            dateInterval,
            query.SubscriptionId,
            query.FromDate,
            query.ToDate
        };
        var command = new CommandDefinition(
            commandText: sql,
            parameters: parameters,
            cancellationToken: cancellationToken
        );
        SubscriptionWithPlanPaymentSummary? result = null;
        var lookup = new Dictionary<Guid, SubscriptionPlanPayment>();
        await connection.QueryAsync<SubscriptionWithPlanPaymentSummary, SubscriptionPlanPayment, PaymentByDate, SubscriptionWithPlanPaymentSummary>(
            command: command,
            (subscription, planPayment, payment) =>
            {
                if (result == null)
                {
                    result = subscription;
                }
                if (lookup.TryGetValue(planPayment.SubscriptionPlanId, out SubscriptionPlanPayment existingPlanPayment))
                {
                    planPayment = existingPlanPayment;
                }
                else
                {
                    lookup.Add(planPayment.SubscriptionPlanId, planPayment);
                    result.PlanPayments.Add(planPayment);
                }
                planPayment.Payments.Add(payment);
                return result;
            },
            splitOn: "SubscriptionPlanId,FromDate"
        );
        if (result == null)
        {
            return Result.Failure<SubscriptionWithPlanPaymentSummary>(SubscriptionErrors.SubscriptionNotFound);
        }
        return result;

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
    public string Price { get; init; }
    public string Duration { get; init; }
    public List<PaymentByDate> Payments { get; init; } = new List<PaymentByDate>();
}
public sealed record PaymentByDate
{
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public long Amount { get; init; }
    public string Currency { get; init; }

}
