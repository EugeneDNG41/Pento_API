using FluentValidation;

namespace Pento.Application.Trades.Sessions.GetAll;

internal sealed class GetTradeSessionsQueryValidator : AbstractValidator<GetTradeSessionsQuery>
{
    public GetTradeSessionsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        RuleFor(x => x.SortOrder)
            .IsInEnum().When(x => x.SortOrder.HasValue)
            .WithMessage("Invalid sort order.");
        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue)
            .WithMessage("Invalid trade session status.");
    }
}

        // Note: Total count query is omitted for brevity; in a real implementation, you
