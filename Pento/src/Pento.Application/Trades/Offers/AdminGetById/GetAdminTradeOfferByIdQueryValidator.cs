using FluentValidation;
using Pento.Application.Trades.Requests.AdminGetById;

namespace Pento.Application.Trades.Offers.AdminGetById;

internal sealed class GetAdminTradeOfferByIdQueryValidator : AbstractValidator<GetAdminTradeOfferByIdQuery>
{
    public GetAdminTradeOfferByIdQueryValidator()
    {
        RuleFor(x => x.TradeOfferId).NotEmpty();
    }
}
