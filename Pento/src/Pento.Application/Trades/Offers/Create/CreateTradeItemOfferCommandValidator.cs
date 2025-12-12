using FluentValidation;

namespace Pento.Application.Trades.Offers.Create;

public sealed class CreateTradeItemOfferCommandValidator
    : AbstractValidator<CreateTradeItemOfferCommand>
{
    public CreateTradeItemOfferCommandValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage("StartDate must be earlier than EndDate.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item must be provided.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.FoodItemId).NotEmpty();
            item.RuleFor(i => i.UnitId).NotEmpty();
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");
        });
    }
}
