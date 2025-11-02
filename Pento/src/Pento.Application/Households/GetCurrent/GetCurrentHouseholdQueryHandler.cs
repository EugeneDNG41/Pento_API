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
    : IQueryHandler<GetCurrentHouseholdQuery, HouseholdResponse>
{
    public async Task<Result<HouseholdResponse>> Handle(GetCurrentHouseholdQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<HouseholdResponse>(HouseholdErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await connectionFactory.OpenConnectionAsync();
        const string sql =
            $"""
                SELECT
                    h.id AS {nameof(HouseholdResponse.Id)},
                    h.name AS {nameof(HouseholdResponse.Name)},
                    h.invite_code AS {nameof(HouseholdResponse.InviteCode)},
                    h.invite_code_expiration_utc AS {nameof(HouseholdResponse.InviteCodeExpirationUtc)},
                    u.id AS {nameof(HouseholdMemberResponse.UserId)},   
                    u.email AS {nameof(HouseholdMemberResponse.Email)},
                    u.first_name AS {nameof(HouseholdMemberResponse.FirstName)},
                    u.last_name AS {nameof(HouseholdMemberResponse.LastName)},
                    u.avatar_url AS {nameof(HouseholdMemberResponse.AvatarUrl)},
                    STRING_AGG(ur.role_name, ',') AS {nameof(HouseholdMemberResponse.Roles)}
                FROM households h
                LEFT JOIN users u ON u.household_id = h.id
                LEFT JOIN user_roles ur ON ur.user_id = u.id 
                WHERE h.id = @HouseholdId
                GROUP BY u.id, h.id;
            """;
        var householdDict = new Dictionary<Guid, HouseholdResponse>();
        CommandDefinition command = new(sql, new { HouseholdId = householdId }, cancellationToken: cancellationToken);
        await connection.QueryAsync<HouseholdResponse, HouseholdMemberResponse, HouseholdResponse>(
            command, (household, member) =>
            {
                if (householdDict.TryGetValue(household.Id, out HouseholdResponse existingHousehold))
                {
                    household = existingHousehold;
                } else
                {
                    householdDict.Add(household.Id, household);
                }
                household!.Members.Add(member);
                return household;
            }, splitOn: nameof(HouseholdMemberResponse.UserId));
        HouseholdResponse? response = householdDict[householdId.Value];
        if (response is null)
        {
            return Result.Failure<HouseholdResponse>(HouseholdErrors.NotFound);
        }
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
