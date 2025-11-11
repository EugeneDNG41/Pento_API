using FluentValidation;

namespace Pento.Application.FoodItems.Consume;

internal sealed class ConsumeFoodItemCommandValidator : AbstractValidator<ConsumeFoodItemCommand>
{
    public ConsumeFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("BaseQuantity must be greater than zero.");
    }
}
