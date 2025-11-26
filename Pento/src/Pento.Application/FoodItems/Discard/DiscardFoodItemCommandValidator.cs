using FluentValidation;

namespace Pento.Application.FoodItems.Discard;

internal sealed class DiscardFoodItemCommandValidator : AbstractValidator<DiscardFoodItemCommand>
{
    public DiscardFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id is required.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
