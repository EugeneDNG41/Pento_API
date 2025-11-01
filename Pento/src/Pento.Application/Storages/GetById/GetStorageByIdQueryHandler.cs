using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.GetById;

internal sealed class GetStorageByIdQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory) 
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
        if (storage.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<StorageResponse>(StorageErrors.ForbiddenAccess);
        }
        return storage;
    }
}
