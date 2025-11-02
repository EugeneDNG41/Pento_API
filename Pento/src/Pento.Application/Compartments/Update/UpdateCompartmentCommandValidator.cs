
using FluentValidation;

namespace Pento.Application.Compartments.Update;

internal sealed class UpdateCompartmentCommandValidator : AbstractValidator<UpdateCompartmentCommand>
{
    public UpdateCompartmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.Notes)
            .MaximumLength(500);
    }
}
