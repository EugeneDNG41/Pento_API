using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.FoodItems.Create;

internal sealed class CreateFoodItemCommandValidator : AbstractValidator<CreateFoodItemCommand>
{
    public CreateFoodItemCommandValidator()
    {
        RuleFor(x => x.FoodRefId).NotEmpty();
        RuleFor(x => x.CompartmentId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.ExpirationDate.ToUniversalTime()).GreaterThan(DateTime.UtcNow);
    }
}
