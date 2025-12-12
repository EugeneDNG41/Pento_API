using FluentValidation;

namespace Pento.Application.Trades.Offers.Cancel;

internal sealed class CancelTradeOfferCommandValidator : AbstractValidator<CancelTradeOfferCommand>
{
    public CancelTradeOfferCommandValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required.");
    }
}
