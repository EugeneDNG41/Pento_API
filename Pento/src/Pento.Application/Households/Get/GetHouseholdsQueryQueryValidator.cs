using FluentValidation;

namespace Pento.Application.Households.Get;

internal sealed class GetHouseholdsQueryQueryValidator : AbstractValidator<GetHouseholdsQuery>
{
    public GetHouseholdsQueryQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0.");
        RuleFor(x => x.FromMember)
            .GreaterThanOrEqualTo(0)
            .WithMessage("From Member must be greater than or equal to 0.")
            .When(x => x.FromMember.HasValue);
        RuleFor(x => x.ToMember)
            .GreaterThanOrEqualTo(0)
            .WithMessage("To Member must be greater than or equal to 0.")
            .When(x => x.ToMember.HasValue);
        RuleFor(x => x.ToMember)
            .GreaterThanOrEqualTo(x => x.FromMember)
            .WithMessage("To Member must be greater than or equal to From Member.")
            .When(x => x.FromMember.HasValue && x.ToMember.HasValue);
    }
}
