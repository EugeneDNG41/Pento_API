using FluentValidation;

namespace Pento.Application.GiveawayClaims.Create;

public sealed class CreateGiveawayClaimCommandValidator
    : AbstractValidator<CreateGiveawayClaimCommand>
{
    public CreateGiveawayClaimCommandValidator()
    {
        RuleFor(x => x.GiveawayPostId)
            .NotEmpty();

        RuleFor(x => x.Message)
            .MaximumLength(500);
    }
}
