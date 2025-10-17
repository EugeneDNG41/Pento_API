using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.PossibleUnits.Create;
internal sealed class CreatePossibleUnitCommandValidator : AbstractValidator<CreatePossibleUnitCommand>
{
    public CreatePossibleUnitCommandValidator()
    {
        RuleFor(c => c.UnitId).NotEmpty();
        RuleFor(c => c.FoodRefId).NotEmpty();
    }
}
