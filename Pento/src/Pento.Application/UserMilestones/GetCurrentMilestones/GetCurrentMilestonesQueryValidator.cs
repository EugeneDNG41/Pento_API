using FluentValidation;

namespace Pento.Application.UserMilestones.GetCurrentMilestones;

internal sealed class GetCurrentMilestonesQueryValidator : AbstractValidator<GetCurrentMilestonesQuery>
{
    public GetCurrentMilestonesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
    }
}
