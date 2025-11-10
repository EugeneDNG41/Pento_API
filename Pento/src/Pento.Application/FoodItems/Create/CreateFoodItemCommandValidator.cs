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
        RuleFor(x => x.FoodReferenceId).NotEmpty().WithMessage("Food reference Id must not be empty.");
        RuleFor(x => x.CompartmentId).NotEmpty().WithMessage("Compartment Id must not be empty.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.ExpirationDateUtc!.Value)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.ExpirationDateUtc.HasValue)
                .WithMessage("Expiration Date must be in the future.");
    }
}
