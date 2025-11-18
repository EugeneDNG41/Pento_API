using FluentValidation;

namespace Pento.Application.Recipes.Reserve;

public sealed class FulfillRecipeReservationCommandValidator
    : AbstractValidator<FulfillRecipeReservationCommand>
{
    public FulfillRecipeReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("ReservationId is required.");

        RuleFor(x => x.NewQuantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.UnitId)
            .NotEmpty()
            .WithMessage("UnitId is required.");
    }
}
