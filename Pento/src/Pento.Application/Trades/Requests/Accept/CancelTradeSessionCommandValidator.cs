using FluentValidation;

namespace Pento.Application.Trades.Requests.Accept;

internal sealed class CancelTradeSessionCommandValidator : AbstractValidator<CancelTradeSessionCommand>
{
    public CancelTradeSessionCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade Session Id is required.");
    }
}
