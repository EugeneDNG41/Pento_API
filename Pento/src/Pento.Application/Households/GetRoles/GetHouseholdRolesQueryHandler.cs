using System.Collections.Generic;
using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Households.GetCurrent;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;

namespace Pento.Application.Households.GetRoles;

internal sealed class GetHouseholdRolesQueryHandler(ISqlConnectionFactory connectionFactory) : IQueryHandler<GetHouseholdRolesQuery, IReadOnlyList<RoleResponse>>
{
    public async Task<Result<IReadOnlyList<RoleResponse>>> Handle(GetHouseholdRolesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        const string sql =
            $"""
                SELECT 
                r.name AS {nameof(RoleResponse.Role)},
                p.name AS {nameof(PermissionResponse.Permission)},
                p.description AS {nameof(PermissionResponse.Description)}
                FROM roles r
                LEFT JOIN role_permissions rp ON rp.role_name = r.name
                LEFT JOIN permissions p ON p.code = rp.permission_code
                WHERE r.type = 'Household'
            """;
        CommandDefinition command = new(sql, cancellationToken: cancellationToken);
        var roleDict = new Dictionary<string, RoleResponse>();
        await connection.QueryAsync<RoleResponse, PermissionResponse, RoleResponse>(
            command, (role, permission) =>
            {
                if (roleDict.TryGetValue(role.Role, out RoleResponse existingRole))
                {
                    role = existingRole;
                }
                else
                {
                    roleDict.Add(role.Role, role);
                }
                role!.Permissions.Add(permission);
                return role;
            }, splitOn: nameof(PermissionResponse.Permission));
        List<RoleResponse> response = roleDict.Values.AsList();
        if (response is null)
        {
            return Result.Failure<IReadOnlyList<RoleResponse>> (RoleErrors.NotFound);
        }
        return response;
    }
}
