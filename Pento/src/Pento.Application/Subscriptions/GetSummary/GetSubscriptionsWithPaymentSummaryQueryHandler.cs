using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Subscriptions.GetSummary;

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
                CAST(p.paid_at AS DATE) AS Date,
                COALESCE(SUM(p.amount_paid), 0) AS Amount,
                p.currency AS Currency
            FROM subscriptions s
            JOIN subscription_plans sp ON sp.subscription_id = s.id
            JOIN payments p ON p.subscription_plan_id = sp.id
            WHERE p.status = 'Paid'
                AND (@ids::uuid[] IS NULL OR s.id = ANY(@ids::uuid[]))
                AND (@IsActive IS NULL OR s.is_active = @IsActive)
                AND (@IsDeleted IS NULL OR s.is_deleted = @IsDeleted)
                AND (@FromDate::date IS NULL OR p.paid_at::date >= @FromDate::date)
                AND (@ToDate::date IS NULL OR p.paid_at::date <= @ToDate::date)
            GROUP BY s.id, s.name, CAST(p.paid_at AS DATE), p.currency
            ORDER BY CAST(p.paid_at AS DATE);
        """;
        var parameters = new
        {
            dateTrunc,
            dateInterval,
            ids = query.SubscriptionIds?.FirstOrDefault() == Guid.Empty ? null : query.SubscriptionIds,
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
            splitOn: "Date"
        );
        return lookup.Values.ToList();
    }
}
