
using MediatR;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Users.GetPermissions;
using Pento.Domain.Abstractions;

namespace Pento.Infrastructure.Authorization;

internal sealed class PermissionService(ISender sender) : IPermissionService
{
    public async Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId)
    {
        return await sender.Send(new GetUserPermissionsQuery(identityId));
    }
}
