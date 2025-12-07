using Pento.Application.Abstractions.Messaging;


namespace Pento.Application.GiveawayClaims.Get;

public sealed record GetGiveawayClaimQuery(Guid ClaimId) : IQuery<GiveawayClaimDetailResponse>;

