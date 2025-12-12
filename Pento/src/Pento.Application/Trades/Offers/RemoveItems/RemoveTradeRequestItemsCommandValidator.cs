using FluentValidation;
using Pento.Application.Trades.Requests.RemoveItems;

namespace Pento.Application.Trades.Offers.RemoveItems;

internal sealed class RemoveTradeRequestItemsCommandValidator : AbstractValidator<RemoveTradeRequestItemsCommand>
{
    public RemoveTradeRequestItemsCommandValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty().WithMessage("Request Id is required.");
        RuleFor(x => x.TradeItemIds).NotEmpty().WithMessage("At least one trade item Id must be provided.");
        RuleForEach(x => x.TradeItemIds)
            .NotEmpty().WithMessage("Trade Item Ids must not contain empty GUIDs.");
    }
}
