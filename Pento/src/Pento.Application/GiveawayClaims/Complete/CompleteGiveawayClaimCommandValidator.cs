using FluentValidation;

namespace Pento.Application.GiveawayClaims.Complete;

public sealed class CompleteGiveawayClaimCommandValidator
    : AbstractValidator<CompleteGiveawayClaimCommand>
{
    public CompleteGiveawayClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
    }
}
