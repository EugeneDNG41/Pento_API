using FluentValidation;

namespace Pento.Application.Users.SignIn;

internal sealed class WebSignInCommandValidator : AbstractValidator<WebSignInCommand>
{
    public WebSignInCommandValidator()
    {
        RuleFor(c => c.Email)
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password cannot be empty.");
    }
}
