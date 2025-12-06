using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Units.Get;
using Pento.Domain.Abstractions;

namespace Pento.Application.Units.GetAll;

internal sealed class GetUnitsQueryHandler(ISqlConnectionFactory connectionFactory) : IQueryHandler<GetUnitsQuery, IReadOnlyList<UnitResponse>>
{
    public async Task<Result<IReadOnlyList<UnitResponse>>> Handle(
        GetUnitsQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            """
            SELECT
                id AS Id,
                name AS Name,
                abbreviation AS Abbreviation,
                to_base_factor AS ToBaseFactor,
                type AS Type
            FROM units
            ORDER BY type ASC
            """;
        CommandDefinition command = new(sql, cancellationToken: cancellationToken);
        List<UnitResponse> units = (await connection.QueryAsync<UnitResponse>(command)).AsList();
        return units;
    }
}
