using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Users.GetAll;

internal sealed class GetUsersQueryQueryHandler(ISqlConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUsersQuery, PagedList<UserPreview>>
{
    public async Task<Result<PagedList<UserPreview>>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetUsersSortBy.HouseholdName => "2",
            GetUsersSortBy.Email => "3",
            GetUsersSortBy.FirstName => "4",
            GetUsersSortBy.LastName => "5",
            GetUsersSortBy.CreatedAt => "6",

            GetUsersSortBy.Id or _ => "1"
        };
        string orderClause = $"ORDER BY {orderBy} {query.SortOrder.ToString()}";
        var filters = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);
        if (query.IsDeleted.HasValue)
        {
            filters.Add("u.is_deleted = @IsDeleted");
            parameters.Add("IsDeleted", query.IsDeleted.Value);
        }
        if (query.SearchText is not null)
        {
            filters.Add("(h.name ILIKE '%' || @SearchText || '%' OR " +
                        "u.first_name ILIKE '%' || @SearchText || '%' OR " +
                        "u.last_name ILIKE '%' || @SearchText || '%' OR " +
                        "u.email ILIKE '%' || @SearchText || '%')");
            parameters.Add("SearchText", query.SearchText);
        }
        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;
        string sql = $"""
            SELECT COUNT(*)
            FROM users u
            LEFT JOIN households h ON u.household_id = h.id
            {whereClause};
            SELECT 
                u.id AS UserId,
                h.name AS HouseholdName,
                u.email AS Email,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.created_at AS CreatedAt,
                u.is_deleted AS IsDeleted
            FROM users u
            LEFT JOIN households h ON u.household_id = h.id
            {whereClause}
            {orderClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalCount = await multi.ReadFirstAsync<int>();
        IEnumerable<UserPreview> users = await multi.ReadAsync<UserPreview>();
        var pagedList = new PagedList<UserPreview>(
            users.ToList(),
            totalCount,
            query.PageNumber,
            query.PageSize);
        return pagedList;
    }
}
