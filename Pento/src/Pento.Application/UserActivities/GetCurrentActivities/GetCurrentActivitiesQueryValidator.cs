using FluentValidation;

namespace Pento.Application.UserActivities.GetCurrentActivities;

internal sealed class GetCurrentActivitiesQueryValidator : AbstractValidator<GetCurrentActivitiesQuery>
{
    public GetCurrentActivitiesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate).When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be less than or equal to To date.");
    }
}
