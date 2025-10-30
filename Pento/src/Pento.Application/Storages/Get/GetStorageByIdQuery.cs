using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Get;
using Pento.Application.Users.GetPermissions;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Get;

public sealed record GetStorageByIdQuery(Guid StorageId) : IQuery<StorageResponse>;

internal sealed class GetStorageByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory) 
    : IQueryHandler<GetStorageByIdQuery, StorageResponse>
{
    public async Task<Result<StorageResponse>> Handle(
        GetStorageByIdQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();
        const string sql =
            $"""
            SELECT
                id AS {nameof(StorageResponse.Id)},
                household_id AS {nameof(StorageResponse.HouseholdId)},
                name AS {nameof(StorageResponse.Name)},
                type AS {nameof(StorageResponse.Type)},
                notes AS {nameof(StorageResponse.Notes)}
            FROM storages
            WHERE id = @StorageId
            """;

        StorageResponse? storage = await connection.QuerySingleOrDefaultAsync<StorageResponse>(
            new CommandDefinition(
                sql,
                new { query.StorageId }, 
                cancellationToken: cancellationToken)
        );

        if (storage is null)
        {
            return Result.Failure<StorageResponse>(StorageErrors.NotFound);
        }

        return storage;
    }

    Task<Result<StorageResponse>> IQueryHandler<GetStorageByIdQuery, StorageResponse>.Handle(GetStorageByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
public sealed record StorageResponse(
    Guid Id,
    Guid HouseholdId,
    string Name,
    string Type,
    string? Notes
);
