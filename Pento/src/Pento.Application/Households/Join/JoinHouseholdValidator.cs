using FluentValidation;

namespace Pento.Application.Households.Join;

internal sealed class JoinHouseholdValidator : AbstractValidator<JoinHouseholdCommand>
{
    public JoinHouseholdValidator()
    {
        RuleFor(x => x.InviteCode)
            .NotEmpty().WithMessage("Invite code must not be empty.");
    }
}
