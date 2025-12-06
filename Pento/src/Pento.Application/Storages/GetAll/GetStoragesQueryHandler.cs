using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Compartments.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Storages.GetAll;

internal sealed class GetCompartmentsQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory) 
    : IQueryHandler<GetStoragesQuery, PagedList<StoragePreview>>
{
    public async Task<Result<PagedList<StoragePreview>>> Handle(
        GetStoragesQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<PagedList<StoragePreview>>(HouseholdErrors.NotInAnyHouseHold);
        }
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            $"""
            SELECT COUNT(*) 
            FROM storages 
            WHERE (@StorageType is NULL OR type = @StorageType )
            AND is_deleted = false 
            AND household_id = @HouseholdId 
            AND name ILIKE '%' || COALESCE(@SearchText, '') || '%';
            SELECT
                s.id AS {nameof(StoragePreview.StorageId)},
                s.name AS {nameof(StoragePreview.StorageName)},
                s.type AS {nameof(StoragePreview.StorageType)},
                (SELECT COUNT(*) FROM compartments c WHERE c.storage_id = s.id AND c.is_deleted = false) AS {nameof(StoragePreview.TotalCompartments)}
            FROM storages s
            WHERE (@StorageType is NULL OR s.type = @StorageType )
            AND s.is_deleted = false 
            AND s.household_id = @HouseholdId
            AND s.name ILIKE '%' || COALESCE(@SearchText, '') || '%'
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;           
            """;
        CommandDefinition command = new(sql, new { StorageType = query.StorageType?.ToString(), Offset = (query.PageNumber - 1) * query.PageSize, query.PageSize, HouseholdId = householdId, query.SearchText }, cancellationToken: cancellationToken);
        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalCompartments = await multi.ReadFirstAsync<int>();
        IEnumerable<StoragePreview> storages = await multi.ReadAsync<StoragePreview>();
        var pagedList = new PagedList<StoragePreview>(
            storages.ToList(),
            totalCompartments,
            query.PageNumber,
            query.PageSize);

        return pagedList;
    }
}
