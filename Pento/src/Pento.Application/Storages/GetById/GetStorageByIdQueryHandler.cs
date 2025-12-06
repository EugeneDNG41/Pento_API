using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Compartments.GetAll;
using Pento.Application.Storages.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.GetById;

internal sealed class GetStorageByIdQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory) 
    : IQueryHandler<GetStorageByIdQuery, StorageDetailResponse>
{
    public async Task<Result<StorageDetailResponse>> Handle(
        GetStorageByIdQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            $"""
            SELECT
                id AS {nameof(StorageResponse.Id)},
                household_id AS {nameof(StorageResponse.HouseholdId)},
                name AS {nameof(StorageResponse.Name)},
                type AS {nameof(StorageResponse.Type)},
                notes AS {nameof(StorageResponse.Notes)}
            FROM storages
            WHERE id = @StorageId and is_deleted = false;

            SELECT COUNT(*) FROM compartments WHERE storage_id = @StorageId AND is_deleted = false;
            SELECT
                c.id AS {nameof(CompartmentPreview.CompartmentId)},
                c.name AS {nameof(CompartmentPreview.CompartmentName)},
                (SELECT COUNT(*) FROM food_items fi WHERE fi.compartment_id = c.id AND fi.is_deleted = false AND  fi.quantity > 0) AS {nameof(CompartmentPreview.TotalItems)}
            FROM compartments c
            WHERE c.storage_id = @StorageId AND c.is_deleted = false
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;
        CommandDefinition command = new(sql, new { query.StorageId, Offset = (query.PageNumber - 1) * query.PageSize, query.PageSize }, cancellationToken: cancellationToken);
        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        StorageResponse? storage = await multi.ReadSingleOrDefaultAsync<StorageResponse>();
        int totalCompartments = await multi.ReadSingleAsync<int>();
        IEnumerable<CompartmentPreview> compartments = await multi.ReadAsync<CompartmentPreview>();
        if (storage is null)
        {
            return Result.Failure<StorageDetailResponse>(StorageErrors.NotFound);
        }
        if (storage.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<StorageDetailResponse>(StorageErrors.ForbiddenAccess);
        }
        var storageDetail = new StorageDetailResponse
        (
            storage.Id,
            storage.HouseholdId,
            storage.Name,
            storage.Type,
            storage.Notes,
            PagedList<CompartmentPreview>.Create(compartments, totalCompartments, query.PageNumber, query.PageSize)
        );
        return storageDetail;
    }
}
