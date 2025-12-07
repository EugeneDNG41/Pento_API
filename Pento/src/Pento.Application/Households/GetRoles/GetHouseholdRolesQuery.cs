using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.GetRoles;

public sealed record GetHouseholdRolesQuery : IQuery<IReadOnlyList<RoleResponse>>;
