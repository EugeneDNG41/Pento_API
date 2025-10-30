using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.GenerateInvite;

public sealed record GenerateInviteCodeCommand(Guid HouseholdId, DateTime? CodeExpirationUtc) : ICommand<string>;
