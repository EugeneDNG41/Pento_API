using FluentValidation;

namespace Pento.Application.Trades.Sessions.GetById;

internal sealed class GetTradeSessionByIdQueryValidator : AbstractValidator<GetTradeSessionByIdQuery>
{
    public GetTradeSessionByIdQueryValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade session Id is required.");
    }
}
