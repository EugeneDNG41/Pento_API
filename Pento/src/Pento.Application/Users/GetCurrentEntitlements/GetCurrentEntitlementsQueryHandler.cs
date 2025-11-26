using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.Users.GetCurrentEntitlements;

internal sealed class GetCurrentEntitlementsQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetCurrentEntitlementsQuery, IReadOnlyList<UserEntitlementResponse>>
{
    public async Task<Result<IReadOnlyList<UserEntitlementResponse>>> Handle(GetCurrentEntitlementsQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            @"
            SELECT
                feature_name AS FeatureName,
                CASE
                    WHEN entitlement_quota IS NULL AND entitlement_reset_per IS NULL
                      THEN 'Unlocked'
                    WHEN entitlement_quota IS NOT NULL AND entitlement_reset_per IS NULL
                      THEN CONCAT(usage_count::text, '/', entitlement_quota::text, ' Total')
                    ELSE CONCAT(usage_count::text, '/', entitlement_quota::text, ' Per ', entitlement_reset_per)
                END AS Entitlement
            FROM user_entitlements
            WHERE user_id = @UserId
              AND (@SearchText IS NULL OR feature_name ILIKE '%' || @SearchText || '%')
              AND (
                    @Available IS NULL OR
                    (@Available = TRUE AND (entitlement_quota IS NULL OR usage_count < entitlement_quota)) OR
                    (@Available = FALSE AND (entitlement_quota IS NOT NULL AND usage_count >= entitlement_quota))
                  );
            ";
        CommandDefinition command = new(sql, new {userContext.UserId, query.SearchText, query.Available }, cancellationToken: cancellationToken);
        List<UserEntitlementResponse> entitlements = (await connection.QueryAsync<UserEntitlementResponse>(command)).AsList();
        return entitlements;
    }
}

