using FluentValidation;

namespace Pento.Application.Compartments.Create;

internal sealed class CreateCompartmentCommandValidator : AbstractValidator<CreateCompartmentCommand>
{
    public CreateCompartmentCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(c => c.Notes)
            .MaximumLength(500);
    }
}
