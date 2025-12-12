using FluentValidation;

namespace Pento.Application.Trades.Offers.UpdateItems;

internal sealed class UpdateTradeOfferItemsCommandValidator : AbstractValidator<UpdateTradeOfferItemsCommand>
{
    public UpdateTradeOfferItemsCommandValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one trade item must be provided.");
        RuleForEach(x => x.Items).SetValidator(new UpdateTradeItemDtoValidator());
    }
}
