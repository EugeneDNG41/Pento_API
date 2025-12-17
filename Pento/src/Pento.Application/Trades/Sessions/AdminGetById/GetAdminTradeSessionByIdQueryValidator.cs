using FluentValidation;

namespace Pento.Application.Trades.Sessions.AdminGetById;

internal sealed class GetAdminTradeSessionByIdQueryValidator : AbstractValidator<GetAdminTradeSessionByIdQuery>
{
    public GetAdminTradeSessionByIdQueryValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty().WithMessage("Trade session Id is required.");
    }
}
