using FluentValidation;

namespace Pento.Application.Trades.Requests.Accept;

internal sealed class ConfirmTradeSessionCommandValidator : AbstractValidator<ConfirmTradeSessionCommand>
{
    public ConfirmTradeSessionCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade Session Id is required.");
    }
}
