using FluentValidation;

namespace Pento.Application.Trades.Offers.AddItems;

internal sealed class AddTradeOfferItemsCommandValidator : AbstractValidator<AddTradeOfferItemsCommand>
{
    public AddTradeOfferItemsCommandValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one trade item must be provided.");
        RuleForEach(x => x.Items).SetValidator(new AddTradeItemDtoValidator());
    }
}
