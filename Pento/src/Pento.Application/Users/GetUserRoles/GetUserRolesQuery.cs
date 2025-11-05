using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.GetUserRoles;

public sealed record GetUserRolesQuery(string IdentityId) : IQuery<UserRolesResponse>;
