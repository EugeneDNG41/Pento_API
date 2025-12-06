using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.UserActivities.GetCurrentActivities;

internal sealed class GetCurrentActivitiesQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetCurrentActivitiesQuery, PagedList<CurrentUserActivityResponse>>
{
    public async Task<Result<PagedList<CurrentUserActivityResponse>>> Handle(GetCurrentActivitiesQuery query, CancellationToken cancellationToken)
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
        if (query.ActivityCodes?.FirstOrDefault() != null)
        {
            filters.Add("ua.activity_code = Any(@ActivityCodes::text[])");
            parameters.Add("ActivityCodes", query.ActivityCodes);
        }
        filters.Add("ua.user_id = @UserId");
        parameters.Add("UserId", userContext.UserId);
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string orderClause = $"ORDER BY {orderBy} {(query.SortOrder != null ? query.SortOrder.ToString() : "ASC")}";
        string sql = $@"
            SELECT 
                a.name,
                a.description,
                ua.performed_on AS PerformedOn
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
        CommandDefinition command = new(sql,parameters,cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        var activities = (await multi.ReadAsync<CurrentUserActivityResponse>()).ToList();
        int totalCount = await multi.ReadFirstAsync<int>();
        return PagedList<CurrentUserActivityResponse>.Create(activities, totalCount, query.PageNumber, query.PageSize);
    }
}
