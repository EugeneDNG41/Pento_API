using FluentValidation;

namespace Pento.Application.FoodItems.Discard;

internal sealed class DiscardFoodItemCommandValidator : AbstractValidator<DiscardFoodItemCommand>
{
    public DiscardFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("BaseQuantity must be greater than zero.");
    }
}
