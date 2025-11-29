using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Giveaways.Claims.Complete;

public sealed record CompleteGiveawayClaimCommand(Guid ClaimId)
    : ICommand<Guid>;
