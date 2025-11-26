using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
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
                description AS Description
            FROM subscriptions
            WHERE id = @SubscriptionId AND is_deleted is false;
            SELECT
                id AS SubscriptionPlanId,
                CONCAT(price_amount::text, ' ', price_currency) AS price,
                CONCAT(duration_value::text, ' ',
                         duration_unit,
                         CASE WHEN COALESCE(duration_value,0) = 1 THEN '' ELSE 's' END
                  ) AS duration
            FROM subscription_plans
            WHERE subscription_id = @SubscriptionId AND is_deleted is false;
            SELECT
                id AS SubscriptionFeatureId,
                feature_name AS FeatureName,
                CASE
                    WHEN entitlement_quota IS NULL AND entitlement_reset_per IS NULL
                      THEN 'Unlocked'
                    WHEN entitlement_quota IS NOT NULL AND entitlement_reset_per IS NULL
                      THEN CONCAT(entitlement_quota::text, ' Total')
                    ELSE CONCAT(entitlement_quota::text, ' Per ', entitlement_reset_per)
                END AS Entitlement
            FROM subscription_features
            WHERE subscription_id = @SubscriptionId AND is_deleted is false;
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
            plans,
            features
        );
        return response;

    }
}
