using FluentValidation;
using Pento.Application.Trades.Requests.Accept;

namespace Pento.Application.Trades.Sessions.Cancel;

internal sealed class CancelTradeSessionCommandValidator : AbstractValidator<CancelTradeSessionCommand>
{
    public CancelTradeSessionCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("TradeAway Session Id is required.");
    }
}
