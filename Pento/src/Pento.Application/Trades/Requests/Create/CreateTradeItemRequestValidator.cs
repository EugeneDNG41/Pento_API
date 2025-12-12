using FluentValidation;

namespace Pento.Application.Trades.Requests.Create;

public sealed class CreateTradeItemRequestValidator
    : AbstractValidator<CreateTradeItemRequestCommand>
{
    public CreateTradeItemRequestValidator()
    {
        RuleFor(x => x.TradeOfferId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();

        RuleForEach(x => x.Items).SetValidator(new AddTradeItemDtoValidator());
    }
}
