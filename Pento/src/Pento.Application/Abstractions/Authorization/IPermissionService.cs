using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Authorization;

public interface IPermissionService
{
    Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId);
}
