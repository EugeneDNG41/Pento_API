using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.UserActivities.GetUserActivities;

internal sealed class GetUserActivitiesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetUserActivitiesQuery, PagedList<UserActivityResponse>>
{
    public async Task<Result<PagedList<UserActivityResponse>>> Handle(GetUserActivitiesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                ua.user_id AS UserId,
                a.name, 
                ua.performed_on AS PerformedOn,
                ua.entity_id AS EntityId
            FROM user_activities ua
            LEFT JOIN activities a ON ua.activity_code = a.code
            WHERE (@SearchTerm IS NULL OR a.name LIKE '%' + @SearchTerm + '%')
              AND (@FromDate IS NULL OR ua.performed_on >= @FromDate)
              AND (@ToDate IS NULL OR ua.performed_on <= @ToDate)
              AND (@UserId IS NULL OR ua.UserId = @UserId)
            ORDER BY ua.performed_on DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            SELECT COUNT(*)
            FROM user_activities ua
            LEFT JOIN activities a ON ua.activity_code = a.code
            WHERE (@SearchTerm IS NULL OR a.name LIKE '%' + @SearchTerm + '%')
              AND (@FromDate IS NULL OR ua.performed_on >= @FromDate)
              AND (@ToDate IS NULL OR ua.performed_on <= @ToDate)
              AND (@UserId IS NULL OR ua.UserId = @UserId)
            ";
        CommandDefinition command = new(
            sql,
            new
            {
                query.SearchTerm,
                query.FromDate,
                query.ToDate,
                query.UserId,
                Offset = (query.PageNumber - 1) * query.PageSize,
                query.PageSize
            },
            cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        var activities = (await multi.ReadAsync<UserActivityResponse>()).ToList();
        int totalCount = await multi.ReadFirstAsync<int>();
        return PagedList<UserActivityResponse>.Create(activities, totalCount, query.PageNumber, query.PageSize);
    }
}
