using FluentValidation;

namespace Pento.Application.FoodReferences.Delete;

internal sealed class DeleteFoodReferenceCommandValidator : AbstractValidator<DeleteFoodReferenceCommand>
{
    public DeleteFoodReferenceCommandValidator()
    {
        RuleFor(x => x.FoodReferenceId)
            .NotEmpty()
            .WithMessage("Food Reference Id is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("Food Reference Id must be a valid GUID.");
    }
}
