using FluentValidation;

namespace Pento.Application.Trades.Sessions.SendMessage;

internal sealed class SendTradeMessageCommandValidator : AbstractValidator<SendTradeMessageCommand>
{
    public SendTradeMessageCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade Session Id is required.");
        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Message must be between 1 and 500 characters.");
    }
}
