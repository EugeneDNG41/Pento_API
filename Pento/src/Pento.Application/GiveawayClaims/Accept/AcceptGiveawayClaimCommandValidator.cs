using FluentValidation;

namespace Pento.Application.GiveawayClaims.Accept;

public sealed class AcceptGiveawayClaimCommandValidator
    : AbstractValidator<AcceptGiveawayClaimCommand>
{
    public AcceptGiveawayClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
    }
}
