using FluentValidation;

namespace Pento.Application.Households.RemoveMember;

internal sealed class RemoveHouseholdMemberCommandValidator : AbstractValidator<RemoveHouseholdMemberCommand>
{
    public RemoveHouseholdMemberCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}
