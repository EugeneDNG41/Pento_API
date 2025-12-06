using System.Data.Common;
using Dapper;
using FluentValidation;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Subscriptions.GetById;
using Pento.Domain.Abstractions;

namespace Pento.Application.Subscriptions.GetAll;

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
                    (@IsActive IS NULL OR is_active = @IsActive) AND
                    is_deleted is false;
            SELECT
                id AS SubscriptionId,
                name AS Name,
                description AS Description,
                is_active AS IsActive
            FROM subscriptions
            WHERE   (@SearchText IS NULL OR 
                    name ILIKE '%' || @SearchText || '%' OR
                    description ILIKE '%' || @SearchText || '%') AND
                    (@IsActive IS NULL OR is_active = @IsActive) AND
                    is_deleted is false
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";
        CommandDefinition subscriptionCommand = new(
            subscriptionSql,
            new
            {
                SearchText = string.IsNullOrWhiteSpace(query.SearchTerm) ? null : query.SearchTerm,
                query.IsActive,
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
                WHERE subscription_id = ANY(@SubscriptionIds) AND is_deleted is false;
                SELECT
                    sf.id AS SubscriptionFeatureId,
                    sf.subscription_id AS SubscriptionId,
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
                WHERE sf.subscription_id = ANY(@SubscriptionIds) AND sf.is_deleted is false;
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
                    subscription.IsActive,
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
