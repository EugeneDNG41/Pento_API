using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.GetPermissions;

internal sealed class GetUserPermissionsQueryHandler(ISqlConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUserPermissionsQuery, PermissionsResponse>
{
    public async Task<Result<PermissionsResponse>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT DISTINCT
                 u.id AS {nameof(UserPermission.UserId)},
                 u.household_id AS {nameof(UserPermission.HouseholdId)},
                 ur.name AS {nameof(UserPermission.Roles)}
             FROM users.users u
             JOIN users.user_roles ur ON ur.user_id = u.id
             JOIN users.role_permissions rp ON rp.role_name = ur.role_name
             WHERE u.identity_id = @IdentityId
             """;

        List<UserPermission> permissions = (await connection.QueryAsync<UserPermission>(sql, request)).AsList();

        if (!permissions.Any())
        {
            return Result.Failure<PermissionsResponse>(UserErrors.NotFound(request.IdentityId));
        }

        return new PermissionsResponse(
            permissions[0].UserId, 
            permissions[0].HouseholdId, 
            permissions.Select(p => p.Roles).ToHashSet());
    }

    internal sealed class UserPermission
    {
        internal Guid UserId { get; init; }
        internal Guid HouseholdId { get; init; }
        internal string Roles { get; init; }
    }
}
