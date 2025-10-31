using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Get;

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
        CommandDefinition command = new(sql, new { query.StorageId }, cancellationToken: cancellationToken);
        StorageResponse? storage = await connection.QuerySingleOrDefaultAsync<StorageResponse>(command);

        if (storage is null)
        {
            return Result.Failure<StorageResponse>(StorageErrors.NotFound);
        }

        return storage;
    }
}
