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
        RuleFor(x => x.Name).MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.");
        RuleFor(x => x.FoodReferenceId).NotEmpty().WithMessage("Food reference Id must not be empty.");
        RuleFor(x => x.CompartmentId).NotEmpty().WithMessage("Compartment Id must not be empty.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("BaseQuantity must be greater than zero.");
        RuleFor(x => x.ExpirationDate!.Value)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))           
                .When(x => x.ExpirationDate.HasValue)
                .WithMessage("Expiration date must not be in the past.");
    }
}
