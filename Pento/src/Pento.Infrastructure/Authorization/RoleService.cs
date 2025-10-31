using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Authorization;

internal sealed class RoleService(IGenericRepository<Role> repository) : IRoleService
{
    public void SetRoles(User user, IEnumerable<Role> roles)
    {
        var rolesList = roles.ToList();
        user.SetRoles(rolesList);
        repository.UpdateRange(roles);
    }
    public async Task<IReadOnlyList<Role>> GetRolesAsync(RoleType? type)
    {
        return (await repository.FindAsync(r => !type.HasValue || r.Type == type.Value)).ToList();
    }
}
