﻿using System;
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
            .MaximumLength(255);
        RuleFor(c => c.DataType)
             .IsInEnum();

        RuleFor(c => c.FoodGroup)
            .IsInEnum();

        RuleFor(c => c.UsdaId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.Brand)
            .MaximumLength(200)
            .When(c => !string.IsNullOrWhiteSpace(c.Brand));

        RuleFor(c => c.Barcode)
            .MaximumLength(100)
            .When(c => !string.IsNullOrWhiteSpace(c.Barcode));

        RuleFor(c => c.TypicalShelfLifeDays_Pantry)
            .GreaterThanOrEqualTo(0);
        RuleFor(c => c.TypicalShelfLifeDays_Fridge)
           .GreaterThanOrEqualTo(0);
        RuleFor(c => c.TypicalShelfLifeDays_Freezer)
           .GreaterThanOrEqualTo(0);

    }


}
