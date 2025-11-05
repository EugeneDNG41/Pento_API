using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Authorization;

public interface IPermissionService
{
    Task<Result<UserPermissionsResponse>> GetUserPermissionsAsync(string identityId);
    Task<Result<UserRolesResponse>> GetUserRolesAsync(string identityId);
}
