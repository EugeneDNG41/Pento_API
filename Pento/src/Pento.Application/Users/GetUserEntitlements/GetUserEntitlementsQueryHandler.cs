using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetCurrentEntitlements;
using Pento.Domain.Abstractions;

namespace Pento.Application.Users.GetUserEntitlements;

internal sealed class GetUserEntitlementsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetUserEntitlementsQuery, IReadOnlyList<UserEntitlementResponse>>
{
    public async Task<Result<IReadOnlyList<UserEntitlementResponse>>> Handle(GetUserEntitlementsQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            @"
            SELECT
                f.name AS FeatureName,
                f.description AS FeatureDescription,
                CASE
                    WHEN ue.quota IS NULL AND ue.reset_period IS NULL
                      THEN 'Unlocked'
                    WHEN ue.quota IS NOT NULL AND ue.reset_period IS NULL
                      THEN CONCAT(ue.usage_count::text, '/', ue.quota::text, ' Total')
                    ELSE CONCAT(ue.usage_count::text, '/', ue.quota::text, ' Per ', ue.reset_period)
                END AS Entitlement,
                CASE
                    WHEN ue.user_subscription_id IS NULL
                      THEN 'Default'
                    ELSE s.name
                END AS FromSubcription
            FROM user_entitlements ue
            LEFT JOIN features f ON ue.feature_code = f.code
            LEFT JOIN user_subscriptions us ON ue.user_subscription_id = us.id
            LEFT JOIN subscriptions s ON us.subscription_id = s.id
            WHERE ue.user_id = @UserId
              AND (@SearchText IS NULL OR f.name ILIKE '%' || @SearchText || '%' OR f.description ILIKE '%' || @SearchText || '%')
              AND (
                    @Available IS NULL OR
                    (@Available = TRUE AND (ue.quota IS NULL OR ue.usage_count < ue.quota)) OR
                    (@Available = FALSE AND (ue.quota IS NOT NULL AND ue.usage_count >= ue.quota))
                  );
            ";
        CommandDefinition command = new(sql, new { query.UserId, query.SearchText, query.Available }, cancellationToken: cancellationToken);
        List<UserEntitlementResponse> entitlements = (await connection.QueryAsync<UserEntitlementResponse>(command)).AsList();
        return entitlements;
    }
}

