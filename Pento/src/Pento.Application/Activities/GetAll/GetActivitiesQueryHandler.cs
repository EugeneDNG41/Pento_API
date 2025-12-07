using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;

namespace Pento.Application.Activities.GetAll;

internal sealed class GetActivitiesQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetActivitiesQuery, IReadOnlyList<ActivityResponse>>
{
    public async Task<Result<IReadOnlyList<ActivityResponse>>> Handle(GetActivitiesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                code AS ActivityCode,
                name AS Name,
                description AS Description
            FROM activities
            WHERE @SearchText IS NULL OR name ILIKE '%' || @SearchText ||  '%' OR description ILIKE '%' || @SearchText || '%' OR code ILIKE '%' || @SearchText || '%'
  
            ORDER BY name;
        ";
        CommandDefinition command = new(sql, new { query.SearchText }, cancellationToken: cancellationToken);
        IEnumerable<ActivityResponse> activities = await connection.QueryAsync<ActivityResponse>(command);
        return activities.ToList();
    }
}
