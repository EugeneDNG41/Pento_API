using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.GetPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionsResponse>;
