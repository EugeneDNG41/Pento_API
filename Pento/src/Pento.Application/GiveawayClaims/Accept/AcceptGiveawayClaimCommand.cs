using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GiveawayClaims.Accept;

public sealed record AcceptGiveawayClaimCommand(Guid ClaimId)
    : ICommand<Guid>;
