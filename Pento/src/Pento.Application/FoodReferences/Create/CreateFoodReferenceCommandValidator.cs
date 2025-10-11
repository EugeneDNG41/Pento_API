using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.FoodReferences.Create;
internal sealed class CreateFoodReferenceCommandValidator : AbstractValidator<CreateFoodReferenceCommand>
{
    public CreateFoodReferenceCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.FoodGroup)
            .NotEmpty()
            .WithMessage("Food group is required.");

        RuleFor(c => c.Barcode)
            .MaximumLength(50)
            .When(c => !string.IsNullOrWhiteSpace(c.Barcode));

        RuleFor(c => c.Brand)
            .MaximumLength(100)
            .When(c => !string.IsNullOrWhiteSpace(c.Brand));

        RuleFor(c => c.TypicalShelfLifeDays)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Shelf life days must be non-negative.");

        RuleFor(c => c.OpenFoodFactsId)
            .MaximumLength(100)
            .When(c => !string.IsNullOrWhiteSpace(c.OpenFoodFactsId));

        RuleFor(c => c.UsdaId)
            .MaximumLength(100)
            .When(c => !string.IsNullOrWhiteSpace(c.UsdaId));
    }
}
