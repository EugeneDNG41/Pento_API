using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Users.GetUserPermissions;

internal sealed class GetUserPermissionsQueryHandler(ISqlConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUserPermissionsQuery, UserPermissionsResponse>
{
    public async Task<Result<UserPermissionsResponse>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
             SELECT DISTINCT
                 u.id AS {nameof(UserPermission.UserId)},
                 u.household_id AS {nameof(UserPermission.HouseholdId)},
                 rp.permission_code AS {nameof(UserPermission.Permission)}
             FROM users u
             LEFT JOIN user_roles ur ON ur.user_id = u.id
             LEFT JOIN role_permissions rp ON rp.role_name = ur.role_name
             WHERE u.identity_id = @IdentityId
             """;
        CommandDefinition command = new(sql, request, cancellationToken: cancellationToken);
        List<UserPermission> permissions = (await connection.QueryAsync<UserPermission>(command)).AsList();

        if (!permissions.Any())
        {
            return Result.Failure<UserPermissionsResponse>(UserErrors.NotFound);
        }
        return new UserPermissionsResponse(permissions[0].UserId, permissions[0].HouseholdId, permissions.Select(p => p.Permission).ToHashSet());
    }

    internal sealed class UserPermission
    {
        internal Guid UserId { get; init; }
        internal Guid? HouseholdId { get; init; }
        internal string Permission { get; init; }
    }
}
