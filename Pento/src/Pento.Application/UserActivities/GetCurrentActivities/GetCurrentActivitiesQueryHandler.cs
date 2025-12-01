using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.UserActivities.GetCurrentActivities;

internal sealed class GetCurrentActivitiesQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetCurrentActivitiesQuery, PagedList<CurrentUserActivityResponse>>
{
    public async Task<Result<PagedList<CurrentUserActivityResponse>>> Handle(GetCurrentActivitiesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql = @"
            SELECT 
                a.name, 
                ua.performed_on AS PerformedOn
            FROM user_activities ua
            LEFT JOIN activities a ON ua.activity_code = a.code
            WHERE (@SearchTerm IS NULL OR a.name LIKE '%' + @SearchTerm + '%')
              AND (@FromDate IS NULL OR ua.performed_on >= @FromDate)
              AND (@ToDate IS NULL OR ua.performed_on <= @ToDate)
              AND ua.UserId = @UserId
            ORDER BY ua.performed_on DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            SELECT COUNT(*)
            FROM user_activities ua
            LEFT JOIN activities a ON ua.activity_code = a.code
            WHERE (@SearchTerm IS NULL OR a.name LIKE '%' + @SearchTerm + '%')
              AND (@FromDate IS NULL OR ua.performed_on >= @FromDate)
              AND (@ToDate IS NULL OR ua.performed_on <= @ToDate)
              AND ua.UserId = @UserId
            ";
        CommandDefinition command = new(
            sql,
            new
            {
                query.SearchTerm,
                query.FromDate,
                query.ToDate,
                userContext.UserId,
                Offset = (query.PageNumber - 1) * query.PageSize,
                query.PageSize
            },
            cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        var activities = (await multi.ReadAsync<CurrentUserActivityResponse>()).ToList();
        int totalCount = await multi.ReadFirstAsync<int>();
        return PagedList<CurrentUserActivityResponse>.Create(activities, totalCount, query.PageNumber, query.PageSize);
    }
}
