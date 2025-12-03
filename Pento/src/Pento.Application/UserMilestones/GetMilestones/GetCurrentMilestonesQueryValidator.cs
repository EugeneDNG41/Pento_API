using FluentValidation;

namespace Pento.Application.UserMilestones.GetMilestones;

internal sealed class GetCurrentMilestonesQueryValidator : AbstractValidator<GetCurrentMilestonesQuery>
{
    public GetCurrentMilestonesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        RuleFor(x => x.SortBy)
            .IsInEnum()
            .WithMessage("Sort By must be a valid enum value.");
        RuleFor(x => x.SortOrder)
            .IsInEnum()
            .WithMessage("Sort Order must be a valid enum value.");
    }
}
