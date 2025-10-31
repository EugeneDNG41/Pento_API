using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Abstractions.Authorization;

public interface IRoleService
{
    Task<IReadOnlyList<Role>> GetRolesAsync(RoleType? type);
    void SetRoles(User user, IEnumerable<Role> roles);
}
