using FluentValidation;

namespace Pento.Application.Trades.Requests.AdminGetById;

internal sealed class GetAdminTradeRequestByIdQueryValidator : AbstractValidator<GetAdminTradeRequestByIdQuery>
{
    public GetAdminTradeRequestByIdQueryValidator()
    {
        RuleFor(x => x.TradeRequestId).NotEmpty();
    }
}
