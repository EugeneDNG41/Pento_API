using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.RemoveMember;

public sealed record RemoveHouseholdMemberCommand(Guid UserId) : ICommand;
