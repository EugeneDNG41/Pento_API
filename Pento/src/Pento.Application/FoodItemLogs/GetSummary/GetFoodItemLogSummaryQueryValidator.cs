using FluentValidation;

namespace Pento.Application.FoodItemLogs.GetSummary;

internal sealed class GetFoodItemLogSummaryQueryValidator : AbstractValidator<GetFoodItemLogSummaryQuery>
{
    public GetFoodItemLogSummaryQueryValidator()
    {
        When(x => x.FromUtc.HasValue && x.ToUtc.HasValue, () =>
        {
            RuleFor(x => x.ToUtc)
                .GreaterThanOrEqualTo(x => x.FromUtc).WithMessage("To Date must be greater than or equal to From Date.");
        });
    }
}
