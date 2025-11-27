using FluentValidation;

namespace Pento.Application.Storages.Delete;

internal sealed class DeleteStorageCommandValidator : AbstractValidator<DeleteStorageCommand>
{
    public DeleteStorageCommandValidator()
    {
        RuleFor(x => x.StorageId)
            .NotEmpty().WithMessage("Storage Code is required.");
    }
}
