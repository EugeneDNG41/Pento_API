using FluentValidation;

namespace Pento.Application.Giveaways.Claims.Complete;

public sealed class CompleteGiveawayClaimCommandValidator
    : AbstractValidator<CompleteGiveawayClaimCommand>
{
    public CompleteGiveawayClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId).NotEmpty();
    }
}
