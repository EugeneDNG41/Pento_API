using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.SetRoles;

public sealed record SetMemberRolesCommand(Guid MemberId, IEnumerable<string> Roles) : ICommand;
