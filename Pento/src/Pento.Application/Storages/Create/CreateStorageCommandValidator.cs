using FluentValidation;

namespace Pento.Application.Storages.Create;

internal sealed class CreateStorageCommandValidator : AbstractValidator<CreateStorageCommand>
{
    public CreateStorageCommandValidator()
    {
        RuleFor(c => c.HouseholdId)
            .NotEmpty();
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(c => c.Notes)
            .MaximumLength(1000);
    }
}
