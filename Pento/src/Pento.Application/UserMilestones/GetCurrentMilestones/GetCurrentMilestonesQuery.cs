using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;

namespace Pento.Application.UserMilestones.GetCurrentMilestones;
public enum UserMilestoneSortBy
{
    Name,
    AchievedOn,
    Progress
}
public enum SortOrder
{
    ASC,
    DESC
}
public sealed record GetCurrentMilestonesQuery(string? SearchTerm, bool? IsAchieved, UserMilestoneSortBy? SortBy, SortOrder SortOrder, int PageNumber, int PageSize) : IQuery<PagedList<CurrentUserMilestonesResponse>>;
internal sealed class GetCurrentMilestonesQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetCurrentMilestonesQuery, PagedList<CurrentUserMilestonesResponse>>
{
    public async Task<Result<PagedList<CurrentUserMilestonesResponse>>> Handle(GetCurrentMilestonesQuery query, CancellationToken cancellationToken)
    {

        string orderBy = query.SortBy switch
        {
            UserMilestoneSortBy.Name => "2",
            UserMilestoneSortBy.AchievedOn => "3",
            UserMilestoneSortBy.Progress => "4",
            _ => "1"
        };
        string orderClause = $"ORDER BY {orderBy} {query.SortOrder.ToString()}";
        var filters = new List<string>
        {
            "m.is_deleted IS false"
        };
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            filters.Add("m.name ILIKE '%' || @SearchTerm || '%'");
        }
        if (query.IsAchieved.HasValue)
        {
            filters.Add("@IsAchieved = (um.achieved_on is not null)");
        }
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string sql = $@"
            SELECT COUNT(*)
            FROM milestones m
            LEFT JOIN user_milestones um
              ON m.id = um.milestone_id
              AND um.user_id = @UserId
            LEFT JOIN LATERAL (
              SELECT 1
              FROM milestone_requirements mr
              WHERE mr.milestone_id = m.id
              LIMIT 1
            ) mr_exists ON true
            {whereClause};
            SELECT
              m.id  AS MileStoneId,
              m.name,
              um.achieved_on AS AchievedOn,
              CASE
                WHEN um.achieved_on IS NOT NULL THEN 100
                WHEN agg.total_quota IS NULL OR agg.total_quota = 0 THEN 0
                ELSE 
                       LEAST(
                         100,
                         FLOOR( (agg.total_achieved::numeric / agg.total_quota::numeric) * 100 )
                       )
         
              END AS progress
            FROM milestones m
            LEFT JOIN user_milestones um
              ON m.id = um.milestone_id
              AND um.user_id = @UserId
            LEFT JOIN LATERAL (
            SELECT
                SUM(mr.quota)                          AS total_quota,
                SUM( LEAST(
                       COALESCE(sub.req_count, 0)::bigint,
                       mr.quota::bigint
                     ) )                                 AS total_achieved
              FROM milestone_requirements mr
              LEFT JOIN LATERAL (
                SELECT
                  CASE
                    WHEN a.type = @StateActivity THEN COUNT(DISTINCT ua2.entity_id)
                    ELSE COUNT(*) 
                  END AS req_count
                FROM user_activities ua2
                JOIN activities a
                  ON a.code = ua2.activity_code
                WHERE ua2.user_id = @UserId
                  AND ua2.activity_code = mr.activity_code
                  AND (
                        mr.within_days IS NULL
                        OR ua2.performed_on >= (current_timestamp - (mr.within_days::text || ' days')::interval)
                      )
	            GROUP BY a.type
              ) sub ON true
              WHERE mr.milestone_id = m.id
              ) agg ON true
            {whereClause}
            {orderClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";
        CommandDefinition command = new(
            sql,
            new
            {
                query.SearchTerm,
                query.IsAchieved,
                userContext.UserId,
                SortBy = query.SortBy.ToString(),
                SortOrder = query.SortOrder.ToString(),
                Offset = (query.PageNumber - 1) * query.PageSize,
                query.PageSize,
                StateActivity = ActivityType.State.ToString()
            },
            cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalCount = await multi.ReadFirstAsync<int>();
        var milestones = (await multi.ReadAsync<CurrentUserMilestonesResponse>()).ToList();
        return PagedList<CurrentUserMilestonesResponse>.Create(milestones, totalCount, query.PageNumber, query.PageSize);
    }
}
public sealed record CurrentUserMilestonesResponse(
    Guid MilestoneId,
    string Name,
    DateTime? AchievedOn,
    decimal Progress);
