using FluentValidation;

namespace Pento.Application.Trades.Offers.AddItems;

internal sealed class AddTradeOfferItemsCommandValidator : AbstractValidator<AddTradeOfferItemsCommand>
{
    public AddTradeOfferItemsCommandValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty().WithMessage("Offer Id is required.");
        RuleFor(x => x.Items)
            .NotNull().WithMessage("Trade Items must not be null.")
            .Must(items => items.Count > 0).WithMessage("At least one trade item must be provided.");
        RuleForEach(x => x.Items).SetValidator(new AddTradeItemDtoValidator());
        RuleFor(x => x.Items)
            .Must(items => items.Select(i => i.FoodItemId).Distinct().Count() == items.Count)
            .WithMessage("Trade contains duplicate items.");
    }
}
