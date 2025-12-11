using FluentValidation;

namespace Pento.Application.Trades.Sessions.RemoveItems;

internal sealed class RemoveTradeSessionItemsCommandValidator : AbstractValidator<RemoveTradeSessionItemsCommand>
{
    public RemoveTradeSessionItemsCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("TradeAway session Id is required.");
        RuleFor(x => x.TradeItemIds)
            .NotNull().WithMessage("TradeAway tradeItem Ids must not be null.")
            .Must(ids => ids.Length > 0).WithMessage("At least one trade tradeItem ID must be provided.");
        RuleForEach(x => x.TradeItemIds)
            .NotEmpty().WithMessage("TradeAway tradeItem Ids must not contain empty GUIDs.");
    }
}
