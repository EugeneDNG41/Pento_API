using FluentValidation;

namespace Pento.Application.Trades.Requests.GetAll;

internal sealed class GetTradeRequestsQueryValidator : AbstractValidator<GetTradeRequestsQuery>
{
    public GetTradeRequestsQueryValidator()
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
