using FluentValidation;

namespace Pento.Application.Storages.Update;

internal sealed class UpdateStorageCommandValidator : AbstractValidator<UpdateStorageCommand>
{
    public UpdateStorageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Storage name must not be empty.")
            .MaximumLength(100)
            .WithMessage("Storage name must not exceed 100 characters.");
        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes must not exceed 500 characters.");
    }
}
