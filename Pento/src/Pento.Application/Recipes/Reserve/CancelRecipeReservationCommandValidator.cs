using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.Recipes.Reserve;
public sealed class CancelRecipeReservationCommandValidator
    : AbstractValidator<CancelRecipeReservationCommand>
{
    public CancelRecipeReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("ReservationId is required.");
    }
}
