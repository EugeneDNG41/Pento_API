using FluentValidation;

namespace Pento.Application.Users.GetAll;

internal sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal to 100.");
        RuleFor(x => x.SortBy)
            .IsInEnum()
            .WithMessage("Invalid sort by value.");
        RuleFor(x => x.SortOrder)
            .IsInEnum()
            .WithMessage("Invalid sort order value.");
    }
}
