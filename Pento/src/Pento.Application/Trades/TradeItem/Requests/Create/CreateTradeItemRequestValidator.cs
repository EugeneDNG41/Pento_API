using FluentValidation;

namespace Pento.Application.Trades.TradeItem.Requests.Create;

public sealed class CreateTradeItemRequestValidator
    : AbstractValidator<CreateTradeItemRequestCommand>
{
    public CreateTradeItemRequestValidator()
    {
        RuleFor(x => x.TradeOfferId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.FoodItemId).NotEmpty();
            item.RuleFor(i => i.UnitId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
