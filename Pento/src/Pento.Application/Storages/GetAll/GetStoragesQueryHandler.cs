using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Storages.GetAll;

internal sealed class GetCompartmentsQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory) 
    : IQueryHandler<GetStoragesQuery, IReadOnlyList<StorageResponse>>
{
    public async Task<Result<IReadOnlyList<StorageResponse>>> Handle(
        GetStoragesQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<IReadOnlyList<StorageResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }
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
            WHERE household_id = @HouseholdId
            """;
        CommandDefinition command = new(sql, new { HouseholdId = householdId }, cancellationToken: cancellationToken);
        List<StorageResponse> storages = (await connection.QueryAsync<StorageResponse>(command)).AsList();
        return storages;
    }
}
