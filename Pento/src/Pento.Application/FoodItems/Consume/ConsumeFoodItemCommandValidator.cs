using FluentValidation;

namespace Pento.Application.FoodItems.Consume;

internal sealed class ConsumeFoodItemCommandValidator : AbstractValidator<ConsumeFoodItemCommand>
{
    public ConsumeFoodItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id is required.");
        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Unit Id is required.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
