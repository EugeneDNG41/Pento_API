using FluentValidation;

namespace Pento.Application.Trades.Requests.GetAll;

internal sealed class GetTradeRequestByIdQueryValidator : AbstractValidator<GetTradeRequestByIdQuery>
{
    public GetTradeRequestByIdQueryValidator()
    {
        RuleFor(x => x.TradeRequestId).NotEmpty();
    }
}
