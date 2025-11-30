using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Compartments.GetAll;
internal sealed class GetCompartmentsQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory) 
    : IQueryHandler<GetCompartmentsQuery, PagedList<CompartmentPreview>>
{
    public async Task<Result<PagedList<CompartmentPreview>>> Handle(
        GetCompartmentsQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<PagedList<CompartmentPreview>>(HouseholdErrors.NotInAnyHouseHold);
        }
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            $"""
            SELECT COUNT(*) FROM compartments WHERE storage_id = @StorageId AND is_deleted = false AND household_id = @HouseholdId AND name ILIKE '%' || COALESCE(@SearchText, '') || '%';
            SELECT
                c.id AS {nameof(CompartmentPreview.CompartmentId)},
                c.name AS {nameof(CompartmentPreview.CompartmentName)},
                (SELECT COUNT(*) FROM food_items fi WHERE fi.compartment_id = c.id AND fi.is_deleted = false AND  fi.quantity > 0) AS {nameof(CompartmentPreview.TotalItems)}
            FROM compartments c
            WHERE c.storage_id = @StorageId AND c.is_deleted = false AND c.household_id = @HouseholdId
                AND c.name ILIKE '%' || COALESCE(@SearchText, '') || '%'
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;           
            """;
        CommandDefinition command = new(sql, new { query.StorageId, Offset = (query.PageNumber - 1) * query.PageSize, query.PageSize, HouseholdId = householdId, query.SearchText }, cancellationToken: cancellationToken);
        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalItems = await multi.ReadFirstAsync<int>();
        IEnumerable<CompartmentPreview> compartments = await multi.ReadAsync<CompartmentPreview>();
        var pagedList = new PagedList<CompartmentPreview>(
            compartments.ToList(),
            totalItems,
            query.PageNumber,
            query.PageSize);

        return pagedList;
    }
}
