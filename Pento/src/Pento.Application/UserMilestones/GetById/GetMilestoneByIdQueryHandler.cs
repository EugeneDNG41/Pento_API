using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;

namespace Pento.Application.UserMilestones.GetById;

internal sealed class GetMilestoneByIdQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetMilestoneByIdQuery, UserMilestoneDetailResponse>
{
    public async Task<Result<UserMilestoneDetailResponse>> Handle(GetMilestoneByIdQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string milestoneSql = @"
            SELECT
              m.id AS MilestoneId,
              m.icon_url AS Icon,
              m.name AS MilestoneName,
              m.description AS MilestoneDescription,
              um.achieved_on AS AchievedOn
            FROM milestones m
            LEFT JOIN user_milestones um
              ON m.id = um.milestone_id
              AND um.user_id = @UserId
            WHERE m.id = @MilestoneId
              AND m.is_deleted IS false;";
        var milestoneParameters = new DynamicParameters();
        milestoneParameters.Add("UserId", userContext.UserId);
        milestoneParameters.Add("MilestoneId", query.MilestoneId);
        UserMilestoneResponse? milestone = await connection.QuerySingleOrDefaultAsync<UserMilestoneResponse>(milestoneSql, milestoneParameters);
        if (milestone is null)
        {
            return Result.Failure<UserMilestoneDetailResponse>(MilestoneErrors.NotFound);
        }
        string requirementsSql = @"
            SELECT
              CASE
                  WHEN mr.within_days IS NULL THEN
                    (SELECT COUNT(*)
                     FROM user_activities ua_all
                     WHERE ua_all.user_id = @UserId
                       AND ua_all.activity_code = mr.activity_code)
                  ELSE
	  	            (SELECT MAX(cnt) AS max_count
			            FROM (
			              SELECT COUNT(*) OVER (
			                ORDER BY performed_on
			                RANGE BETWEEN CURRENT ROW AND (CONCAT(mr.within_days::text,  ' days'))::interval FOLLOWING
			              ) AS cnt
			              FROM user_activities
			              WHERE user_id = @UserId
			                AND activity_code = mr.activity_code
			            ) t)
                END AS Progress,
              mr.quota AS Quota,
              CASE
                    WHEN mr.within_days IS NULL THEN 'Unlimited'
                    ELSE CONCAT('Within ', mr.within_days::text, ' Day',
                        CASE 
                            WHEN COALESCE(mr.within_days,0) = 1 THEN '' ELSE 's' 
                        END)
              END AS TimeFrame
            FROM milestone_requirements mr
            WHERE mr.milestone_id = @MilestoneId;";
        var requirementsParameters = new DynamicParameters();
        requirementsParameters.Add("UserId", userContext.UserId);
        requirementsParameters.Add("MilestoneId", query.MilestoneId);
        var requirements = (await connection.QueryAsync<UserMilestoneRequirement>(requirementsSql, requirementsParameters)).ToList();
        var response = new UserMilestoneDetailResponse(milestone, requirements);
        return Result.Success(response);
    }
}
