using FluentValidation;

namespace Pento.Application.Trades.Sessions.AddItems;

internal sealed class AddTradeItemDtoValidator : AbstractValidator<AddTradeItemDto>
{
    public AddTradeItemDtoValidator()
    {
        RuleFor(x => x.FoodItemId)
            .NotEmpty().WithMessage("Food tradeItem Id is required.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Unit Id is required.");
    }
}
