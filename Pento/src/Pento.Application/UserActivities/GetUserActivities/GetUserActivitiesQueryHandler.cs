using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.UserActivities.GetCurrentActivities;
using Pento.Domain.Abstractions;

namespace Pento.Application.UserActivities.GetUserActivities;

internal sealed class GetUserActivitiesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetUserActivitiesQuery, PagedList<UserActivityResponse>>
{
    public async Task<Result<PagedList<UserActivityResponse>>> Handle(GetUserActivitiesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetUserActivitiesSortBy.Name => "a.name",
            GetUserActivitiesSortBy.PerformedOn => "ua.performed_on",
            GetUserActivitiesSortBy.Description => "a.description",
            _ => "ua.id"
        };

        var filters = new List<string>();
        var parameters = new DynamicParameters();
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            filters.Add("(a.name ILIKE @SearchTerm OR a.description ILIKE @SearchTerm)");
            parameters.Add("SearchTerm", $"%{query.SearchTerm}%");
        }
        if (query.FromDate.HasValue)
        {
            filters.Add("ua.performed_on >= @FromDate");
            parameters.Add("FromDate", query.FromDate.Value);
        }
        if (query.ToDate.HasValue)
        {
            filters.Add("ua.performed_on <= @ToDate");
            parameters.Add("ToDate", query.ToDate.Value);
        }
        if (query.UserIds?.FirstOrDefault() != Guid.Empty)
        {
            filters.Add("ua.user_id = Any(@UserId::uuid[])");
            parameters.Add("UserId", query.UserIds);
        }
        if (query.ActivityCodes?.FirstOrDefault() != null)
        {
            filters.Add("ua.activity_code = Any(@ActivityCodes::text[])");
            parameters.Add("ActivityCodes", query.ActivityCodes);
        }
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string orderClause = $"ORDER BY {orderBy} {(query.SortOrder != null ? query.SortOrder.ToString() : "ASC")}";
        string sql = $@"
            SELECT 
                ua.user_id AS UserId,
                a.name, 
                a.description AS Description,
                ua.performed_on AS PerformedOn,
                ua.entity_id AS EntityId
            FROM user_activities ua
            LEFT JOIN activities a ON ua.activity_code = a.code
            {whereClause}
            {orderClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            SELECT COUNT(*)
            FROM user_activities ua
            LEFT JOIN activities a ON ua.activity_code = a.code
            {whereClause}
            ";
        CommandDefinition command = new(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        var activities = (await multi.ReadAsync<UserActivityResponse>()).ToList();
        int totalCount = await multi.ReadFirstAsync<int>();
        return PagedList<UserActivityResponse>.Create(activities, totalCount, query.PageNumber, query.PageSize);
    }
}
