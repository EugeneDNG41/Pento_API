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
                 string_agg(ur.role_name, ',') AS {nameof(UserPermission.Roles)}
             FROM users u
             LEFT JOIN user_roles ur ON ur.user_id = u.id
             WHERE u.identity_id = @IdentityId
             GROUP BY u.id, u.household_id;
             """;

        UserPermission? permission = await connection.QuerySingleOrDefaultAsync<UserPermission>(sql, request);

        if (permission is null)
        {
            return Result.Failure<PermissionsResponse>(UserErrors.NotFound(request.IdentityId));
        }
        HashSet<string> roles = string.IsNullOrEmpty(permission.Roles) ?
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) : 
            permission.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return new PermissionsResponse(
            permission.UserId, 
            permission.HouseholdId,
            roles);
    }

    internal sealed class UserPermission
    {
        internal Guid UserId { get; init; }
        internal Guid? HouseholdId { get; init; }
        internal string? Roles { get; init; }
    }
}
