using FluentValidation;

namespace Pento.Application.FoodItems.Consume;

internal sealed class ConsumeFoodItemCommandValidator : AbstractValidator<ConsumeFoodItemCommand>
{
    public ConsumeFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Code is required.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
