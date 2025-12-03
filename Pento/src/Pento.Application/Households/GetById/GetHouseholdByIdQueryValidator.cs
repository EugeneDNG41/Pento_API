using FluentValidation;

namespace Pento.Application.Households.GetById;

internal sealed class GetHouseholdByIdQueryValidator : AbstractValidator<GetHouseholdByIdQuery>
{
    public GetHouseholdByIdQueryValidator()
    {
        RuleFor(x => x.HouseholdId).NotEmpty().WithMessage("Household Id is required.");
    }
}
