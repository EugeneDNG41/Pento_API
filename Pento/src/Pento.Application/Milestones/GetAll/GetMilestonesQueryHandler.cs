using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Milestones.GetById;
using Pento.Domain.Abstractions;

namespace Pento.Application.Milestones.GetAll;

internal sealed class GetMilestonesQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetMilestonesQuery, PagedList<MilestoneResponse>>
{
    public async Task<Result<PagedList<MilestoneResponse>>> Handle(GetMilestonesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sqlBuilder =
            @"
            SELECT COUNT(*) FROM milestones
            WHERE is_deleted is false 
                AND (name ILIKE '%' || COALESCE(@SearchTerm, '') || '%' OR description ILIKE '%' || COALESCE(@SearchTerm, '') || '%')
                AND (@IsActive IS NULL OR is_active = @IsActive);
            SELECT
                id,
                name,
                description,
                is_active AS IsActive
            FROM milestones
            WHERE is_deleted is false 
                AND (name ILIKE '%' || COALESCE(@SearchTerm, '') || '%' OR description ILIKE '%' || COALESCE(@SearchTerm, '') || '%')
                AND (@IsActive IS NULL OR is_active = @IsActive)
            ORDER BY name 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";
        CommandDefinition command = new(
            sqlBuilder.ToString(),
            new
            {
                query.SearchTerm,
                query.IsActive,
                Offset = (query.PageNumber - 1) * query.PageSize,
                query.PageSize
            },
            cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalItems = await multi.ReadFirstAsync<int>();
        IEnumerable<MilestoneResponse> milestones = await multi.ReadAsync<MilestoneResponse>();
        var pagedList = new PagedList<MilestoneResponse>(
            milestones.ToList(),
            totalItems,
            query.PageNumber,
            query.PageSize);
        return pagedList;
    }
}
