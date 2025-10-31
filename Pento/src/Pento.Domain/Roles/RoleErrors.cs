using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Roles;

public static class RoleErrors
{
    public static readonly Error NotFound = Error.NotFound("Role.NotFound", "Role not found.");
    public static readonly Error MustHaveOne = Error.Problem("Role.MustHaveOne", "At least one role must be assigned.");
}
