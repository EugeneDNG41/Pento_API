using FluentValidation;

namespace Pento.Application.Trades;

internal sealed class UpdateTradeItemDtoValidator : AbstractValidator<UpdateTradeItemDto>
{
    public UpdateTradeItemDtoValidator()
    {
        RuleFor(x => x.TradeItemId)
            .NotEmpty().WithMessage("Trade Item Id is required.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Unit Id is required.");
    }
}
