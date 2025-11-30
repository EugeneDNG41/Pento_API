using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;

namespace Pento.Application.Milestones.GetById;

internal sealed class GetMilestoneByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetMilestoneByIdQuery, MilestoneDetailResponse>
{
    public async Task<Result<MilestoneDetailResponse>> Handle(GetMilestoneByIdQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            @"
            SELECT
                id,
                name,
                description,
                is_active AS IsActive
            FROM milestones
            WHERE id = @Id AND is_deleted is false;

            SELECT
                id,
                a.name AS Activity,
                quota,
                CASE
                    WHEN within_days IS NULL THEN 'Unlimited'
                    ELSE CONCAT('Within', within_days::text, ' ', time_frame_unit,
                        CASE 
                            WHEN COALESCE(within_days,0) = 1 THEN '' ELSE 's' 
                        END)
                END
                AS TimeFrame
            FROM milestone_requirements
            JOIN activities a ON milestone_requirements.activity_code = a.code
            WHERE milestone_id = @Id AND is_deleted is false;
            ";
        CommandDefinition command = new(sql, new { query.Id }, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        MilestoneResponse? milestone = await multi.ReadSingleOrDefaultAsync<MilestoneResponse>();
        if (milestone == null)
        {
            return Result.Failure<MilestoneDetailResponse>(MilestoneErrors.NotFound);
        }
        IReadOnlyList<MilestoneRequirementResponse> requirements = (await multi.ReadAsync<MilestoneRequirementResponse>()).ToList();
        return new MilestoneDetailResponse(milestone, requirements);
    }
}
