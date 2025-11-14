using FluentValidation;

namespace Pento.Application.FoodItemLogs.Search;

internal sealed class SearchFoodItemLogQueryValidator : AbstractValidator<SearchFoodItemLogQuery>
{
    public SearchFoodItemLogQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        When(x => x.FromUtc.HasValue && x.ToUtc.HasValue, () =>
        {
            RuleFor(x => x.ToUtc)
                .GreaterThanOrEqualTo(x => x.FromUtc).WithMessage("To Date must be greater than or equal to From Date.");
        });
    }
}
