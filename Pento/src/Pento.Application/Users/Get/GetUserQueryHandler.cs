using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using static Pento.Application.Users.GetPermissions.GetUserPermissionsQueryHandler;

namespace Pento.Application.Users.Get;

internal sealed class GetUserQueryHandler(ISqlConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 u.id AS {nameof(UserResponse.Id)},
                 u.household_id AS {nameof(UserResponse.HouseholdId)},
                 u.avatar_url AS {nameof(UserResponse.AvatarUrl)},
                 u.email AS {nameof(UserResponse.Email)},
                 u.first_name AS {nameof(UserResponse.FirstName)},
                 u.last_name AS {nameof(UserResponse.LastName)},
                 u.created_at AS {nameof(UserResponse.CreatedAt)},
                 string_agg(ur.role_name, ',') AS {nameof(UserResponse.Roles)}
             FROM users u
             LEFT JOIN user_roles ur ON ur.user_id = id
             WHERE id = @UserId
             GROUP BY u.id;
             """;

        UserResponse? user = await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, request);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound);
        }
        return user;
    }
}
