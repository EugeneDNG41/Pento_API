using FluentValidation;

namespace Pento.Application.FoodItems.MoveToCompartment;

internal sealed class MoveFoodItemToCompartmentCommandValidator : AbstractValidator<MoveFoodItemToCompartmentCommand>
{
    public MoveFoodItemToCompartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.NewCompartmentId)
            .NotEmpty().WithMessage("Compartment Id must not be empty.");
    }
}
