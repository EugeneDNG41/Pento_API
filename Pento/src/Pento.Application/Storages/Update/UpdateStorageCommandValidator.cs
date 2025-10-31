using FluentValidation;

namespace Pento.Application.Storages.Update;

internal sealed class UpdateStorageCommandValidator : AbstractValidator<UpdateStorageCommand>
{
    public UpdateStorageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}
