using FluentValidation;

namespace Pento.Application.Milestones.GetAll;

internal sealed class GetAdminMilestonesQueryValidator : AbstractValidator<GetAdminMilestonesQuery>
{
    public GetAdminMilestonesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        RuleFor(x => x.SortBy)
            .IsInEnum()
            .WithMessage("Invalid sort by value.");
        RuleFor(x => x.SortOrder)
            .IsInEnum()
            .WithMessage("Invalid sort order value.");
    }
}
