using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.Application.Households.GetRoles;

internal sealed class GetHouseholdRolesQueryHandler(ISqlConnectionFactory connectionFactory) : IQueryHandler<GetHouseholdRolesQuery, IReadOnlyList<RoleResponse>>
{
    public async Task<Result<IReadOnlyList<RoleResponse>>> Handle(GetHouseholdRolesQuery query, CancellationToken cancellationToken)
    {
        using DbConnection connection = await connectionFactory.OpenConnectionAsync();
        const string sql =
            $"""
                SELECT 
                name AS {nameof(RoleResponse.Role)}
                FROM roles
                WHERE type = 'Household'
            """;
        CommandDefinition command = new(sql, cancellationToken: cancellationToken);
        List<RoleResponse> roles = (await connection.QueryAsync<RoleResponse>(command)).AsList();
        if (roles.Count == 0)
        {
            return Result.Failure<IReadOnlyList<RoleResponse>>(RoleErrors.NotFound);
        }
        return roles;
    }
}
