using System.Data.Common;
using Dapper;
using FluentValidation;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Subscriptions.GetById;

internal sealed class GetSubscriptionsQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetSubscriptionsQuery, PagedList<SubscriptionDetailResponse>>
{
    public async Task<Result<PagedList<SubscriptionDetailResponse>>> Handle(GetSubscriptionsQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string subscriptionSql =
            @"
            SELECT COUNT(*) 
            FROM subscriptions
            WHERE   (@SearchText IS NULL OR 
                    name ILIKE '%' || @SearchText || '%' OR
                    description ILIKE '%' || @SearchText || '%') AND
                    is_deleted is false;
            SELECT
                id AS SubscriptionId,
                name AS Name,
                description AS Description
            FROM subscriptions
            WHERE   (@SearchText IS NULL OR 
                    name ILIKE '%' || @SearchText || '%' OR
                    description ILIKE '%' || @SearchText || '%') AND
                    is_deleted is false
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";
        CommandDefinition subscriptionCommand = new(
            subscriptionSql,
            new
            {
                SearchText = string.IsNullOrWhiteSpace(query.SearchTerm) ? null : query.SearchTerm,
                Offset = (query.PageNumber - 1) * query.PageSize,
                query.PageSize
            });
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(subscriptionCommand);
        int totalCount = await multi.ReadSingleAsync<int>();
        var subscriptions = (await multi.ReadAsync<SubscriptionResponse>()).ToList();
        if (totalCount > 0 && subscriptions.Count > 0)
        {
            const string planAndFeatureSql =
                @"
                SELECT
                    id AS SubscriptionPlanId,
                    subscription_id AS SubscriptionId,
                    CONCAT(price_amount::text, ' ', price_currency) AS price,
                    CONCAT(duration_value::text, ' ',
                             duration_unit,
                             CASE WHEN COALESCE(duration_value,0) = 1 THEN '' ELSE 's' END
                      ) AS duration
                FROM subscription_plans
                WHERE subscription_id = ANY(@SubscriptionIds) AND is_deleted is false;
                SELECT
                    id AS SubscriptionFeatureId,
                    subscription_id AS SubscriptionId,
                    feature_name AS FeatureName,
                    CASE
                        WHEN entitlement_quota IS NULL AND entitlement_reset_per IS NULL
                          THEN 'Unlocked'
                        WHEN entitlement_quota IS NOT NULL AND entitlement_reset_per IS NULL
                          THEN CONCAT(entitlement_quota::text, ' Total')
                        ELSE CONCAT(entitlement_quota::text, ' Per ', entitlement_reset_per)
                    END AS Entitlement
                FROM subscription_features
                WHERE subscription_id = ANY(@SubscriptionIds) AND is_deleted is false;
                ";
            CommandDefinition planAndFeatureCommand = new(planAndFeatureSql, new { SubscriptionIds = subscriptions.Select(s => s.SubscriptionId).ToArray() });
            using SqlMapper.GridReader planAndFeatureMulti = await connection.QueryMultipleAsync(planAndFeatureCommand);
            var plans = (await planAndFeatureMulti.ReadAsync<SubscriptionPlanResponseWithSubscriptionId>()).ToList();
            var features = (await planAndFeatureMulti.ReadAsync<SubscriptionFeatureResponseWithSubscriptionId>()).ToList();
            var subscriptionDetails = new List<SubscriptionDetailResponse>();
            foreach (SubscriptionResponse? subscription in subscriptions)
            {
                IReadOnlyList<SubscriptionPlanResponse> subscriptionPlans = plans.Where(p => p.SubscriptionId == subscription.SubscriptionId)
                    .Select(p => new SubscriptionPlanResponse
                    {
                        SubscriptionPlanId = p.SubscriptionPlanId,
                        Price = p.Price,
                        Duration = p.Duration
                    })
                    .ToList();
                IReadOnlyList<SubscriptionFeatureResponse> subscriptionFeatures = features.Where(f => f.SubscriptionId == subscription.SubscriptionId)
                    .Select(f => new SubscriptionFeatureResponse
                    {
                        SubscriptionFeatureId = f.SubscriptionFeatureId,
                        FeatureName = f.FeatureName,
                        Entitlement = f.Entitlement
                    })
                    .ToList();
                subscriptionDetails.Add(new SubscriptionDetailResponse(
                    subscription.SubscriptionId,
                    subscription.Name,
                    subscription.Description,
                    subscriptionPlans,
                    subscriptionFeatures));
            }
            var pagedList = new PagedList<SubscriptionDetailResponse>(
                subscriptionDetails,
                totalCount,
                query.PageNumber,
                query.PageSize);
            return pagedList;
        }
        var emptyPagedList = new PagedList<SubscriptionDetailResponse>(
            new List<SubscriptionDetailResponse>(),
            totalCount,
            query.PageNumber,
            query.PageSize);
        return emptyPagedList;
    }
}
