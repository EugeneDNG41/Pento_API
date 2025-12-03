

using FluentValidation;

namespace Pento.Application.Households.Delete;

internal sealed class DeleteHouseholdCommandValidator : AbstractValidator<DeleteHouseholdCommand>
{
    public DeleteHouseholdCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Household Id is required.");
    }
}
