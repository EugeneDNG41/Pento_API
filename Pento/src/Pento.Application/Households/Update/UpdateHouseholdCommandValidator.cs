using FluentValidation;

namespace Pento.Application.Households.Update;

internal sealed class UpdateHouseholdCommandValidator : AbstractValidator<UpdateHouseholdCommand>
{
    public UpdateHouseholdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Household name is required.")
            .MaximumLength(200).WithMessage("Household name must not exceed 100 characters.");
    }
}
