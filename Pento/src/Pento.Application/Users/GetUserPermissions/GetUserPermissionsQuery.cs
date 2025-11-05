using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<UserPermissionsResponse>;
