using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.GetPermissions;
using Pento.Domain.Abstractions;

namespace Pento.Infrastructure.Authorization;

internal sealed class PermissionService(IQueryHandler<GetUserPermissionsQuery, PermissionsResponse> handler) : IPermissionService
{
    public async Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId)
    {
        return await handler.Handle(new GetUserPermissionsQuery(identityId), default);
    }
}
