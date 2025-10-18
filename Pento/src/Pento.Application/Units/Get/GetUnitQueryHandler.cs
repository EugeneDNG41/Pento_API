﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Units;

namespace Pento.Application.Units.Get;
internal sealed class GetUnitQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetUnitQuery, UnitResponse>
{
    public async Task<Result<UnitResponse>> Handle(GetUnitQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                id AS Id,
                name AS Name,
                abbreviation AS Abbreviation,
                to_base_factor AS ToBaseFactor,
                created_on_utc AS CreatedOnUtc,
                updated_on_utc AS UpdatedOnUtc
            FROM units
            WHERE id = @UnitId
            """;

        UnitResponse? unit = await connection.QuerySingleOrDefaultAsync<UnitResponse>(sql, request);

        if (unit is null)
        {
            return Result.Failure<UnitResponse>(UnitErrors.NotFound(request.UnitId));
        }

        return unit;
    }
}
