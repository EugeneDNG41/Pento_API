using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.SetRoles;

public sealed record SetMemberRolesCommand(Guid MemberId, IEnumerable<string> Roles) : ICommand;
