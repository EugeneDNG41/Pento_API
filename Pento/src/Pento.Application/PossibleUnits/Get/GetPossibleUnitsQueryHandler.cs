using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.PossibleUnits;

namespace Pento.Application.PossibleUnits.Get;
internal sealed class GetPossibleUnitsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetPossibleUnitsQuery, IReadOnlyList<PossibleUnitResponse>>
{
    public async Task<Result<IReadOnlyList<PossibleUnitResponse>>> Handle(
        GetPossibleUnitsQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                id AS Id,
                unit_id AS UnitId,
                foodref_id AS FoodRefId,
                is_default AS IsDefault,
                created_on_utc AS CreatedOnUtc,
                updated_on_utc AS UpdatedOnUtc
            FROM possible_units
            WHERE foodref_id = @FoodRefId
            """;

        var possibleUnits = (await connection.QueryAsync<PossibleUnitResponse>(sql, request)).ToList();

        if (!possibleUnits.Any())
        {
            return Result.Failure<IReadOnlyList<PossibleUnitResponse>>(
                PossibleUnitErrors.InvalidFoodReference(request.FoodRefId)
            );
        }

        return possibleUnits;
    }
}
