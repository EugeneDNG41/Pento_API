using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.Features.GetAll;

internal sealed class GetFeaturesQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetFeaturesQuery, IReadOnlyList<FeatureResponse>>
{
    public async Task<Result<IReadOnlyList<FeatureResponse>>> Handle(GetFeaturesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                code AS FeatureCode,
                name AS Name,
                description AS Description,
                CASE
                    WHEN default_quota IS NULL AND default_reset_period IS NULL
                       THEN 'Locked'
                    WHEN default_quota IS NOT NULL AND default_reset_period IS NULL
                       THEN CONCAT(default_quota::text, ' Total')
                    ELSE CONCAT(default_quota::text, ' Per ', default_reset_period)
                END AS DefaultEntitlement                
            FROM features
            WHERE (@SearchText IS NULL OR name ILIKE '%' || @SearchText ||  '%' OR description ILIKE '%' || @SearchText || '%' OR code ILIKE '%' || @SearchText || '%')
            ORDER BY name;
        ";
        CommandDefinition command = new(sql, new { query.SearchText}, cancellationToken: cancellationToken);
        IEnumerable<FeatureResponse> features = await connection.QueryAsync<FeatureResponse>(command);
        return features.ToList();
    }
}
