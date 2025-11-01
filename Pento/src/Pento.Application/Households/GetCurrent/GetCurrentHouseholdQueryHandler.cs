using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Households.GetCurrent;

internal sealed class GetCurrentHouseholdQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory connectionFactory)
    : IQueryHandler<GetCurrentHouseholdQuery, HouseholdDetailResponse>
{
    public async Task<Result<HouseholdDetailResponse>> Handle(GetCurrentHouseholdQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<HouseholdDetailResponse>(UserErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await connectionFactory.OpenConnectionAsync();
        const string sql =
            $"""
                SELECT
                    h.id AS {nameof(HouseholdMembersFlatResult.HouseholdId)},
                    h.name AS {nameof(HouseholdMembersFlatResult.HouseholdName)},
                    h.invite_code AS {nameof(HouseholdMembersFlatResult.InviteCode)},
                    h.invite_code_expiration_utc AS {nameof(HouseholdMembersFlatResult.InviteCodeExpirationUtc)},
                    u.id AS {nameof(HouseholdMembersFlatResult.UserId)},   
                    u.email AS {nameof(HouseholdMembersFlatResult.Email)},
                    u.first_name AS {nameof(HouseholdMembersFlatResult.FirstName)},
                    u.last_name AS {nameof(HouseholdMembersFlatResult.LastName)},
                    u.avatar_url AS {nameof(HouseholdMembersFlatResult.AvatarUrl)},
                    STRING_AGG(ur.role_name, ',') AS {nameof(HouseholdMembersFlatResult.Roles)}
                FROM households h
                LEFT JOIN users u ON u.household_id = h.id
                LEFT JOIN user_roles ur ON ur.user_id = u.id 
                WHERE h.id = @HouseholdId
                GROUP BY u.id, h.id;
            """;
        CommandDefinition command = new(sql, new { HouseholdId = householdId }, cancellationToken: cancellationToken);
        var result = (await connection.QueryAsync<HouseholdMembersFlatResult>(command)).ToList();
        if (!result.Any())
        {
            return Result.Failure<HouseholdDetailResponse>(HouseholdErrors.NotFound);
        }
        HouseholdDetailResponse response = result.GroupBy(x => x.HouseholdId, members => members, (k, v) => new HouseholdDetailResponse(
            k,
            v.First().HouseholdName,
            v.First().InviteCode,
            v.First().InviteCodeExpirationUtc,
            v.Select(m => new HouseholdMember(
                m.UserId,
                m.Email,
                m.FirstName,
                m.LastName,
                m.AvatarUrl,
                m.Roles)).ToList()
        )).First();
        return response;
    }
    internal sealed record HouseholdMembersFlatResult(
        Guid HouseholdId,
        string HouseholdName,
        string? InviteCode,
        DateTime? InviteCodeExpirationUtc,
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        Uri? AvatarUrl,
        string Roles);
}
