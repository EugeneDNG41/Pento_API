using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;

namespace Pento.Application.Milestones.GetById;

internal sealed class GetAdminMilestoneByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetAdminMilestoneByIdQuery, AdminMilestoneDetailResponse>
{
    public async Task<Result<AdminMilestoneDetailResponse>> Handle(GetAdminMilestoneByIdQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            @"
            SELECT
                id,
                icon_url AS Icon,
                name,
                description,
                is_active AS IsActive,
                (SELECT COUNT(*) FROM user_milestones um WHERE um.milestone_id = m.id) AS EarnedCount,
                is_deleted AS IsDeleted
            FROM milestones m
            WHERE id = @Id;

            SELECT
                id,
                a.name AS Activity,
                quota,
                CASE
                    WHEN within_days IS NULL THEN 'Unlimited'
                    ELSE CONCAT('Within', within_days::text, ' Day',
                        CASE 
                            WHEN COALESCE(within_days,0) = 1 THEN '' ELSE 's' 
                        END)
                END
                AS TimeFrame
            FROM milestone_requirements
            JOIN activities a ON milestone_requirements.activity_code = a.code
            WHERE milestone_id = @Id;
            ";
        CommandDefinition command = new(sql, new { query.Id }, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        AdminMilestoneResponse? milestone = await multi.ReadSingleOrDefaultAsync<AdminMilestoneResponse>();
        if (milestone == null)
        {
            return Result.Failure<AdminMilestoneDetailResponse>(MilestoneErrors.NotFound);
        }
        IReadOnlyList<AdminMilestoneRequirementResponse> requirements = (await multi.ReadAsync<AdminMilestoneRequirementResponse>()).ToList();
        return new AdminMilestoneDetailResponse(milestone, requirements);
    }
}
