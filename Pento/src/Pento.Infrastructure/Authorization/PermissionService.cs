using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetUserPermissions;
using Pento.Application.Users.GetUserRoles;
using Pento.Domain.Abstractions;

namespace Pento.Infrastructure.Authorization;

internal sealed class PermissionService(
    IQueryHandler<GetUserRolesQuery, UserRolesResponse> roleHandler,
    IQueryHandler<GetUserPermissionsQuery, UserPermissionsResponse> permissionHandler) : IPermissionService
{
    public async Task<Result<UserRolesResponse>> GetUserRolesAsync(string identityId)
    {
        return await roleHandler.Handle(new GetUserRolesQuery(identityId), default);
    }
    public async Task<Result<UserPermissionsResponse>> GetUserPermissionsAsync(string identityId)
    {
        return await permissionHandler.Handle(new GetUserPermissionsQuery(identityId), default);
    }
}
