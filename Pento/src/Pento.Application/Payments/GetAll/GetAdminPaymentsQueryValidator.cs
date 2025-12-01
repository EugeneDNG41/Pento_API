using FluentValidation;

namespace Pento.Application.Payments.GetAll;

internal sealed class GetAdminPaymentsQueryValidator : AbstractValidator<GetAdminPaymentsQuery>
{
    public GetAdminPaymentsQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        RuleFor(q => q.FromAmount)
            .GreaterThanOrEqualTo(0).When(q => q.FromAmount.HasValue).WithMessage("From amount must be greater than or equal to 0.");
        RuleFor(q => q.ToAmount)
            .GreaterThanOrEqualTo(0).When(q => q.ToAmount.HasValue).WithMessage("To amount must be greater than or equal to 0.");
        RuleFor(q => q)
            .Must(q => !q.FromAmount.HasValue || !q.ToAmount.HasValue || q.FromAmount <= q.ToAmount)
            .WithMessage("From amount must be less than or equal to To amount.");
        RuleFor(q => q)
            .Must(q => !q.FromDate.HasValue || !q.ToDate.HasValue || q.FromDate <= q.ToDate)
            .WithMessage("From date must be less than or equal to To date.");
    }
}
