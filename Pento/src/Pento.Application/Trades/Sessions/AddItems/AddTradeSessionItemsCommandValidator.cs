using FluentValidation;
using Pento.Application.Trades.Requests.Accept;

namespace Pento.Application.Trades.Sessions.AddItems;

internal sealed class AddTradeSessionItemsCommandValidator : AbstractValidator<AddTradeSessionItemsCommand>
{
    public AddTradeSessionItemsCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade session Id is required.");
        RuleFor(x => x.Items)
            .NotNull().WithMessage("Trade Items must not be null.")
            .Must(items => items.Count > 0).WithMessage("At least one trade tradeItem must be provided.");
        RuleForEach(x => x.Items).SetValidator(new AddTradeItemDtoValidator());
        RuleFor(x => x.Items)
            .Must(items => items.Select(i => i.FoodItemId).Distinct().Count() == items.Count)
            .WithMessage("Trade contains duplicate items.");
    }
}
