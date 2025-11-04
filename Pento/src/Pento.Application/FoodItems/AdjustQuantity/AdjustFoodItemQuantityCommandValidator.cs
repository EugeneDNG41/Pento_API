using FluentValidation;

namespace Pento.Application.FoodItems.AdjustQuantity;

internal sealed class AdjustFoodItemQuantityCommandValidator : AbstractValidator<AdjustFoodItemQuantityCommand>
{
    public AdjustFoodItemQuantityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
