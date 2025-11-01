using FluentValidation;

namespace Pento.Application.Households.SetRoles;

internal sealed class SetMemberRolesCommandValidator : AbstractValidator<SetMemberRolesCommand>
{
    
    public SetMemberRolesCommandValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.Roles)
            .NotNull()
            .Must(rs => rs.Any())
                .WithMessage("At least one role is required.")
            .Must(r => r.Distinct().Count() == r.Count())
                .WithMessage("Duplicate roles are not allowed.");
        RuleForEach(x => x.Roles!)
            .NotEmpty().WithMessage("Role cannot be empty.");
    }
}
