using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.GetSummaryById;

internal sealed class GetSubscriptionByIdWithPlanPaymentSummary2QueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetSubscriptionWithPlanPaymentSummaryById2Query, SubscriptionWithPlanPaymentSummary2>
{
    public async Task<Result<SubscriptionWithPlanPaymentSummary2>> Handle(GetSubscriptionWithPlanPaymentSummaryById2Query query, CancellationToken cancellationToken)
    {

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
                CAST(p.paid_at AS DATE) AS Date,
                COALESCE(SUM(p.amount_paid), 0) AS Amount,
                p.currency AS Currency
            FROM subscriptions s
            JOIN subscription_plans sp ON sp.subscription_id = s.id
            JOIN payments p ON p.subscription_plan_id = sp.id
            WHERE p.status = 'Paid'
                AND s.id = @SubscriptionId
              AND (@FromDate::date IS NULL OR p.paid_at::date >= @FromDate::date)
              AND (@ToDate::date IS NULL OR p.paid_at::date <= @ToDate::date)
            GROUP BY s.id, s.name, sp.id, CAST(p.paid_at AS DATE), p.amount_paid, p.currency
            ORDER BY CAST(p.paid_at AS DATE);
         """;
        var parameters = new
        {
            query.SubscriptionId,
            query.FromDate,
            query.ToDate
        };
        var command = new CommandDefinition(
            commandText: sql,
            parameters: parameters,
            cancellationToken: cancellationToken
        );
        SubscriptionWithPlanPaymentSummary2? result = null;
        var lookup = new Dictionary<Guid, SubscriptionPlanPayment2>();
        await connection.QueryAsync<SubscriptionWithPlanPaymentSummary2, SubscriptionPlanPayment2, PaymentByDate2, SubscriptionWithPlanPaymentSummary2>(
            command: command,
            (subscription, planPayment, payment) =>
            {
                if (result == null)
                {
                    result = subscription;
                }
                if (lookup.TryGetValue(planPayment.SubscriptionPlanId, out SubscriptionPlanPayment2 existingPlanPayment))
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
            splitOn: "SubscriptionPlanId,Date"
        );
        if (result == null)
        {
            return Result.Failure<SubscriptionWithPlanPaymentSummary2>(SubscriptionErrors.SubscriptionNotFound);
        }
        return result;

    }
}
