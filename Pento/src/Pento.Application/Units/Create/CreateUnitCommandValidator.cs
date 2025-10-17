using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.Units.Create;
internal sealed class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Unit name is required.")
            .MaximumLength(50);

        RuleFor(c => c.Abbreviation)
            .MaximumLength(20);

        RuleFor(c => c.ToBaseFactor)
            .GreaterThan(0).WithMessage("Conversion factor must be greater than zero.");
    }
}
