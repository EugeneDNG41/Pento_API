
using FluentValidation;

namespace Pento.Application.Compartments.Update;

internal sealed class UpdateCompartmentCommandValidator : AbstractValidator<UpdateCompartmentCommand>
{
    public UpdateCompartmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Compartment name is required.")
            .MaximumLength(100)
            .WithMessage("Compartment name must not exceed 100 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes must not exceed 500 characters.");
    }
}
