using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.GenerateInvite;

public sealed record GenerateInviteCodeCommand(DateTime? CodeExpiration) : ICommand<string>;
