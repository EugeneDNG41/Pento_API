using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.UserSubscriptions.GetCurrentSubscriptionById;
using Pento.Application.UserSubscriptions.GetCurrentSubscriptions;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.UserSubscriptions.GetUserSubscriptionById;

internal sealed class GetUserSubscriptionByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetUserSubscriptionByIdQuery, UserSubscriptionDetailResponse>
{
    public async Task<Result<UserSubscriptionDetailResponse>> Handle(GetUserSubscriptionByIdQuery query, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            @"
            SELECT
                us.id AS UserSubscriptionId,
                s.id AS SubscriptionId,
                s.name AS SubscriptionName,
                us.status AS Status,
                us.start_date AS StartDate,
                us.end_date AS EndDate,
                us.paused_date AS PausedDate,
                us.cancelled_date AS CancelledDate,
                CASE
                    WHEN us.paused_date IS NOT NULL AND us.remaining_days_after_pause IS NOT NULL
                    THEN CONCAT(us.remaining_days_after_pause::text, ' ', 'day',
                        CASE 
                            WHEN COALESCE(us.remaining_days_after_pause,0) = 1 THEN '' ELSE 's' 
                        END)
                    WHEN us.cancelled_date IS NOT NULL
                    THEN 'Cancelled'
                    WHEN us.end_date IS NOT NULL AND us.cancelled_date IS NULL
                    THEN CONCAT((us.end_date::date - current_date)::text, ' ', 'day',
                        CASE 
                            WHEN COALESCE(us.end_date::date - current_date) = 1 THEN '' ELSE 's' 
                        END)
                    ELSE 'Lifetime'
                END AS Duration
            FROM user_subscriptions us
            INNER JOIN subscriptions s ON s.id = us.subscription_id
            WHERE us.id = @UserSubscriptionId;

            SELECT
                f.name AS FeatureName,
                f.description AS FeatureDescription,
                CASE
                    WHEN ue.quota IS NULL AND ue.reset_period IS NULL
                      THEN 'Unlocked'
                    WHEN ue.quota IS NOT NULL AND ue.reset_period IS NULL
                      THEN CONCAT(ue.usage_count::text, '/', ue.quota::text, ' Total')
                    ELSE CONCAT(ue.usage_count::text, '/', ue.quota::text, ' Per ', ue.reset_period)
                END AS Entitlement
            FROM user_entitlements ue
            LEFT JOIN features f ON ue.feature_code = f.code
            WHERE ue.user_subscription_id = @UserSubscriptionId;
            ";
        CommandDefinition command = new(sql, new { query.UserSubscriptionId }, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        UserSubscriptionResponse? subscription = await multi.ReadSingleOrDefaultAsync<UserSubscriptionResponse>();
        if (subscription is null)
        {
            return Result.Failure<UserSubscriptionDetailResponse>(SubscriptionErrors.UserSubscriptionNotFound);
        }
        var entitlements = (await multi.ReadAsync<UserEntitlementBySubscription>()).ToList();
        return new UserSubscriptionDetailResponse(subscription, entitlements);
    }
}
