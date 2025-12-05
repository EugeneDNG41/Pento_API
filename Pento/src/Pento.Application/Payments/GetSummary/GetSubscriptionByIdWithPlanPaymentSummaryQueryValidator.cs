using FluentValidation;

namespace Pento.Application.Payments.GetSummary;

internal sealed class GetSubscriptionByIdWithPlanPaymentSummaryQueryValidator : AbstractValidator<GetSubscriptionByIdWithPlanPaymentSummaryQuery>
{
    public GetSubscriptionByIdWithPlanPaymentSummaryQueryValidator()
    {
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From Date must be less than or equal to To Date.");
        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("To Date must be greater than or equal to From Date.");
    }
}
