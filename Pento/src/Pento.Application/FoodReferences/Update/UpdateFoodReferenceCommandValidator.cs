using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.FoodReferences.Update;
internal sealed class UpdateFoodReferenceCommandValidator : AbstractValidator<UpdateFoodReferenceCommand>
{
    public UpdateFoodReferenceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("FoodReference ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.FoodGroup)
            .NotEmpty()
            .WithMessage("Food group is required.");

        RuleFor(x => x.PublishedOnUtc)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Published date cannot be in the future.");

        RuleFor(x => x.TypicalShelfLifeDays_Pantry).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TypicalShelfLifeDays_Fridge).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TypicalShelfLifeDays_Freezer).GreaterThanOrEqualTo(0);
    }
}
