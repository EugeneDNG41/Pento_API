using FluentValidation;
using Pento.Application.Trades.Requests.Accept;

namespace Pento.Application.Trades.Sessions.Confirm;

internal sealed class ConfirmTradeSessionCommandValidator : AbstractValidator<ConfirmTradeSessionCommand>
{
    public ConfirmTradeSessionCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade Session Id is required.");
    }
}
