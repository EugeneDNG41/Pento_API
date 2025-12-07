using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.Join;

public sealed record JoinHouseholdCommand(string InviteCode) : ICommand;
