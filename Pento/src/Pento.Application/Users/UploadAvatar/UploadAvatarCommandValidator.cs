using FluentValidation;

namespace Pento.Application.Users.UploadAvatar;

internal sealed class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    public UploadAvatarCommandValidator()
    {

        RuleFor(x => x.File)
            .NotNull().WithMessage("File must be provided.")
            .Must(file => file != null && file.Length > 0).WithMessage("File is required.");
    }
}
