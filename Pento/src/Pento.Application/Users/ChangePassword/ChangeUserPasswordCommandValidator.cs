using FluentValidation;

namespace Pento.Application.Users.ChangePassword;
#pragma warning disable S125 // Sections of code should not be commented out
internal sealed class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()

    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.");
            //.MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
            //.Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
            //.Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter.")
            //.Matches("[0-9]").WithMessage("New password must contain at least one digit.")
            //.Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character.");
        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword).WithMessage("Confirm new password must match the new password.");
    }

}
