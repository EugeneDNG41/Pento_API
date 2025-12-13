using FluentValidation;

namespace Pento.Application.Recipes.Reserve;

internal sealed class CancelRecipeReservationCommandValidator
    : AbstractValidator<CancelRecipeReservationCommand>
{
    public CancelRecipeReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("ReservationId is required.");
    }
}
