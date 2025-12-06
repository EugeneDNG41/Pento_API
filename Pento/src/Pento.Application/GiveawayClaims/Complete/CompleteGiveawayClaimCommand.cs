using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GiveawayClaims.Complete;

public sealed record CompleteGiveawayClaimCommand(Guid ClaimId)
    : ICommand<Guid>;
