using FluentValidation;
using Pento.Application.Trades.Offers.RemoveItems;

namespace Pento.Application.Trades.Requests.RemoveItems;

internal sealed class RemoveTradeOfferItemsCommandValidator : AbstractValidator<RemoveTradeOfferItemsCommand>
{
    public RemoveTradeOfferItemsCommandValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required.");
        RuleFor(x => x.TradeItemIds).NotEmpty().WithMessage("At least one trade item Id must be provided.");
        RuleForEach(x => x.TradeItemIds)
            .NotEmpty().WithMessage("Trade Item Ids must not contain empty GUIDs.");
    }
}
