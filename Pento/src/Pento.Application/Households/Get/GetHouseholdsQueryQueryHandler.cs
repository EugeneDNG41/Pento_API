using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Households.Get;

internal sealed class GetHouseholdsQueryQueryHandler(
    ISqlConnectionFactory connectionFactory)
    : IQueryHandler<GetHouseholdsQuery, PagedList<HouseholdPreview>>
{
    public async Task<Result<PagedList<HouseholdPreview>>> Handle(GetHouseholdsQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetHouseholdsSortBy.Name => "h.name",
            GetHouseholdsSortBy.Members => "Members",
            _ => "h.id"
        };
        string orderClause = $"ORDER BY {orderBy} {query.SortOrder}";
        var filters = new List<string>();
        var parameters = new DynamicParameters();
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            filters.Add("h.name ILIKE @SearchTerm OR h.invite_code ILIKE @SearchTerm");
            parameters.Add("SearchTerm", $"%{query.SearchTerm}%");
        }
        if (query.IsDeleted.HasValue)
        {
            filters.Add("h.is_deleted = @IsDeleted");
            parameters.Add("IsDeleted", query.IsDeleted.Value);
        }
        if (query.FromMember.HasValue)
        {
            filters.Add("(SELECT COUNT(u.id) FROM users u WHERE u.household_id = h.id) >= @FromMember");
            parameters.Add("FromMember", query.FromMember.Value);
        }
        if (query.ToMember.HasValue)
        {
            filters.Add("(SELECT COUNT(u.id) FROM users u WHERE u.household_id = h.id) <= @ToMember");
            parameters.Add("ToMember", query.ToMember.Value);
        }
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string sql =
            $"""
                SELECT
                    h.id,
                    h.name,
                    h.invite_code AS InviteCode,
                    COUNT(u.id) AS Members,
                    h.is_deleted AS IsDeleted
                FROM households h
                LEFT JOIN users u ON u.household_id = h.id
                {whereClause}
                GROUP BY h.id
                {orderClause}              
                LIMIT @PageSize OFFSET @Offset;
                SELECT COUNT(*) FROM households h LEFT JOIN users u ON u.household_id = h.id {whereClause};
            """;
        
        CommandDefinition command = new(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        IEnumerable<HouseholdPreview> households = await multi.ReadAsync<HouseholdPreview>();
        int totalCount = await multi.ReadFirstAsync<int>();
        return PagedList<HouseholdPreview>.Create(
            households,
            totalCount,
            query.PageNumber,
            query.PageSize);
    }
}
