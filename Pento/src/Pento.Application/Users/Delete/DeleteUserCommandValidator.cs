using FluentValidation;

namespace Pento.Application.Users.Delete;

internal sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User Id is required.");
    }
}

