using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Milestones.GetById;
using Pento.Domain.Abstractions;

namespace Pento.Application.Milestones.GetAll;

internal sealed class GetAdminMilestonesQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetAdminMilestonesQuery, PagedList<AdminMilestoneResponse>>
{
    public async Task<Result<PagedList<AdminMilestoneResponse>>> Handle(GetAdminMilestonesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetAdminMilestoneSortBy.Name => "3",
            GetAdminMilestoneSortBy.EarnedCount => "6",
            GetAdminMilestoneSortBy.Id or _ => "1"
        };
        string orderClause = $"ORDER BY {orderBy} {query.SortOrder}";
        var filters = new List<string>();
        var parameters = new DynamicParameters();
        if (query.IsActive.HasValue)
        {
            filters.Add("is_active = @IsActive");
            parameters.Add("IsActive", query.IsActive.Value);
        }
        if (query.IsDeleted.HasValue) {
            filters.Add("is_deleted = @IsDeleted");
            parameters.Add("IsDeleted", query.IsDeleted.Value);
        }
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            filters.Add("(name ILIKE '%' || @SearchTerm || '%' OR description ILIKE '%' || @SearchTerm || '%')");
            parameters.Add("SearchTerm", query.SearchTerm);
        }
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string sqlBuilder =
            $@"
            SELECT COUNT(*) FROM milestones
            {whereClause};
            SELECT
                m.id,
                m.icon_url AS Icon,
                name,
                description,
                is_active AS IsActive,
                (SELECT COUNT(*) FROM user_milestones um WHERE um.milestone_id = m.id) AS EarnedCount,
                is_deleted AS IsDeleted
            FROM milestones m
            {whereClause}
            {orderClause}
            LIMIT @PageSize OFFSET @Offset;
            ";
        CommandDefinition command = new(
            sqlBuilder.ToString(),
            new
            {
                query.SearchTerm,
                query.IsActive,
                query.IsDeleted,
                Offset = (query.PageNumber - 1) * query.PageSize,
                query.PageSize
            },
            cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalItems = await multi.ReadFirstAsync<int>();
        IEnumerable<AdminMilestoneResponse> milestones = await multi.ReadAsync<AdminMilestoneResponse>();
        var pagedList = new PagedList<AdminMilestoneResponse>(
            milestones.ToList(),
            totalItems,
            query.PageNumber,
            query.PageSize);
        return pagedList;
    }
}
