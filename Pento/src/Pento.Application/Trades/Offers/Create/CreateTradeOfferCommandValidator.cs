using FluentValidation;

namespace Pento.Application.Trades.Offers.Create;

public sealed class CreateTradeOfferCommandValidator
    : AbstractValidator<CreateTradeOfferCommand>
{
    public CreateTradeOfferCommandValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .WithMessage("StartDate must be earlier than EndDate");

        RuleFor(x => x.PickupOption)
            .IsInEnum()
            .WithMessage("Invalid pickup option");
    }
}
