using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GiveawayClaims.Create;

public sealed record CreateGiveawayClaimCommand(
    Guid GiveawayPostId,
    string? Message
) : ICommand<Guid>;
