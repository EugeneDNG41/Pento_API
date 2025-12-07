using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Subscriptions;

namespace Pento.Application.Subscriptions.GetById;

internal sealed class GetSubscriptionByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetSubscriptionByIdQuery, SubscriptionDetailResponse>
{
    public async Task<Result<SubscriptionDetailResponse>> Handle(GetSubscriptionByIdQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            @"
            SELECT
                id AS SubscriptionId,
                name AS Name,
                description AS Description,
                is_active AS IsActive
            FROM subscriptions
            WHERE id = @SubscriptionId AND is_deleted is false;
            SELECT
                id AS SubscriptionPlanId,
                CONCAT(amount::text, ' ', currency) AS price,
                CASE
                    WHEN duration_in_days IS NULL THEN 'Lifetime'
                    ELSE CONCAT(duration_in_days::text, ' ', 'day',
                        CASE 
                            WHEN COALESCE(duration_in_days,0) = 1 THEN '' ELSE 's' 
                        END)
                END
                AS duration
            FROM subscription_plans
            WHERE subscription_id = @SubscriptionId AND is_deleted is false;
            SELECT
                sf.id AS SubscriptionFeatureId,
                f.name AS FeatureName,
                CASE
                    WHEN sf.quota IS NULL AND sf.reset_period IS NULL
                      THEN 'Unlocked'
                    WHEN sf.quota IS NOT NULL AND sf.reset_period IS NULL
                      THEN CONCAT(sf.quota::text, ' Total')
                    ELSE CONCAT(sf.quota::text, ' Per ', sf.reset_period)
                END AS Entitlement
            FROM subscription_features sf
            LEFT JOIN features f ON sf.feature_code = f.code
            WHERE sf.subscription_id = @SubscriptionId AND sf.is_deleted is false;
            ";
        CommandDefinition command = new(sql, new { query.SubscriptionId });
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        SubscriptionResponse? subscription = await multi.ReadSingleOrDefaultAsync<SubscriptionResponse>();
        if (subscription is null)
        {
            return Result.Failure<SubscriptionDetailResponse>(SubscriptionErrors.SubscriptionNotFound);
        }
        var plans = (await multi.ReadAsync<SubscriptionPlanResponse>()).ToList();
        var features = (await multi.ReadAsync<SubscriptionFeatureResponse>()).ToList();
        var response = new SubscriptionDetailResponse(
            subscription.SubscriptionId,
            subscription.Name,
            subscription.Description,
            subscription.IsActive,
            plans,
            features
        );
        return response;

    }
}
