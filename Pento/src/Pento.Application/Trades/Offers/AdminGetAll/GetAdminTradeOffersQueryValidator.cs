using FluentValidation;

namespace Pento.Application.Trades.Offers.AdminGetAll;

internal sealed class GetAdminTradeOffersQueryValidator : AbstractValidator<GetAdminTradeOffersQuery>
{
    public GetAdminTradeOffersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        RuleFor(x => x.SortBy)
            .IsInEnum().WithMessage("Invalid sort by option.")
            .When(x => x.SortBy.HasValue);
        RuleFor(x => x.SortOrder)
            .IsInEnum().WithMessage("Invalid sort order option.")
            .When(x => x.SortOrder.HasValue);
    }
}
