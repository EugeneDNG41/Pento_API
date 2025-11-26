using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;

namespace Pento.Application.Features.GetAll;

internal sealed class GetFeaturesQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetFeaturesQuery, IReadOnlyList<FeatureResponse>>
{
    public async Task<Result<IReadOnlyList<FeatureResponse>>> Handle(GetFeaturesQuery request, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                name AS Name
            FROM Features
            ORDER BY name;
        ";
        CommandDefinition command = new(sql, cancellationToken: cancellationToken);
        IEnumerable<FeatureResponse> features = await connection.QueryAsync<FeatureResponse>(command);
        return features.ToList();
    }
}
