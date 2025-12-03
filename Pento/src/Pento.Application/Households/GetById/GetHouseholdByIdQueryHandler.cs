using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;

namespace Pento.Application.Households.GetById;

internal sealed class GetHouseholdByIdQueryHandler(
    ISqlConnectionFactory connectionFactory)
    : IQueryHandler<GetHouseholdByIdQuery, HouseholdAdminResponse>
{
    public async Task<Result<HouseholdAdminResponse>> Handle(GetHouseholdByIdQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            $"""
                SELECT
                    h.id AS {nameof(HouseholdAdminResponse.Id)},
                    h.name AS {nameof(HouseholdAdminResponse.Name)},
                    h.invite_code AS {nameof(HouseholdAdminResponse.InviteCode)},
                    h.invite_code_expiration_utc AS {nameof(HouseholdAdminResponse.InviteCodeExpirationUtc)},
                    h.is_deleted AS {nameof(HouseholdAdminResponse.IsDeleted)},
                    u.id AS {nameof(HouseholdMemberAdminResponse.UserId)},   
                    u.email AS {nameof(HouseholdMemberAdminResponse.Email)},
                    u.first_name AS {nameof(HouseholdMemberAdminResponse.FirstName)},
                    u.last_name AS {nameof(HouseholdMemberAdminResponse.LastName)},
                    u.avatar_url AS {nameof(HouseholdMemberAdminResponse.AvatarUrl)},
                    STRING_AGG(ur.role_name, ',') AS {nameof(HouseholdMemberAdminResponse.Roles)},
                    u.is_deleted AS {nameof(HouseholdMemberAdminResponse.IsDeleted)}
                FROM households h
                LEFT JOIN users u ON u.household_id = h.id
                LEFT JOIN user_roles ur ON ur.user_id = u.id 
                WHERE h.id = @HouseholdId
                GROUP BY u.id, h.id;
            """;

        CommandDefinition command = new(sql, new { query.HouseholdId }, cancellationToken: cancellationToken);
        var householdDict = new Dictionary<Guid, HouseholdAdminResponse>();
        await connection.QueryAsync<HouseholdAdminResponse, HouseholdMemberAdminResponse, HouseholdAdminResponse>(
            command, (household, member) =>
            {
                if (householdDict.TryGetValue(household.Id, out HouseholdAdminResponse existingHousehold))
                {
                    household = existingHousehold;
                }
                else
                {
                    householdDict.Add(household.Id, household);
                }
                household!.Members.Add(member);
                return household;
            }, splitOn: nameof(HouseholdMemberAdminResponse.UserId));
        HouseholdAdminResponse? response = householdDict[query.HouseholdId];
        if (response is null)
        {
            return Result.Failure<HouseholdAdminResponse>(HouseholdErrors.NotFound);
        }
        return response;
    }
}
