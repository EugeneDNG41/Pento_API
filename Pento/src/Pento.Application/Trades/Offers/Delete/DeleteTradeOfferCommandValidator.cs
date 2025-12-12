using FluentValidation;
using Pento.Application.Trades.Offers.Cancel;

namespace Pento.Application.Trades.Offers.Delete;

internal sealed class DeleteTradeOfferCommandValidator : AbstractValidator<CancelTradeOfferCommand>
{
    public DeleteTradeOfferCommandValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required.");
    }
}
