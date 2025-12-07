using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Units;

namespace Pento.Application.Units.Get;

internal sealed class GetUnitQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetUnitQuery, UnitResponse>
{
    public async Task<Result<UnitResponse>> Handle(GetUnitQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            """
            SELECT
                id AS Id,
                name AS Name,
                abbreviation AS Abbreviation,
                to_base_factor AS ToBaseFactor,
                type AS Type
            FROM units
            WHERE id = @UnitId
            """;
        CommandDefinition command = new(sql, request, cancellationToken: cancellationToken);
        UnitResponse? unit = await connection.QuerySingleOrDefaultAsync<UnitResponse>(command);

        if (unit is null)
        {
            return Result.Failure<UnitResponse>(UnitErrors.NotFound);
        }

        return unit;
    }
}
