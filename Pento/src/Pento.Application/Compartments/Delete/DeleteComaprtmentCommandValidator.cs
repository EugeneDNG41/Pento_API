using FluentValidation;

namespace Pento.Application.Compartments.Delete;

internal sealed class DeleteComaprtmentCommandValidator : AbstractValidator<DeleteCompartmentCommand>
{
    public DeleteComaprtmentCommandValidator()
    {
        RuleFor(x => x.CompartmentId)
            .NotEmpty().WithMessage("Storage Code is required.");
    }
}
