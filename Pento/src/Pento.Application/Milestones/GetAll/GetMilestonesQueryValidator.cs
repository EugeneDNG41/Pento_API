using FluentValidation;

namespace Pento.Application.Milestones.GetAll;

internal sealed class GetMilestonesQueryValidator : AbstractValidator<GetMilestonesQuery>
{
    public GetMilestonesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
    }
}
