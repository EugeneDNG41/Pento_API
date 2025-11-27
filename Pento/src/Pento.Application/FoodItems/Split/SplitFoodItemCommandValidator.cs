using FluentValidation;

namespace Pento.Application.FoodItems.Split;

internal sealed class SplitFoodItemCommandValidator : AbstractValidator<SplitFoodItemCommand>
{
    public SplitFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id is required.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
