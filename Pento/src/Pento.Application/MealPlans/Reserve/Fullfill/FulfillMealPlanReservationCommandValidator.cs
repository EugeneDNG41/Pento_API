using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.MealPlans.Reserve.Fullfill;
public sealed class FulfillMealPlanReservationCommandValidator
    : AbstractValidator<FulfillMealPlanReservationCommand>
{
    public FulfillMealPlanReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId).NotEmpty();
        RuleFor(x => x.NewQuantity).GreaterThan(0);
        RuleFor(x => x.UnitId).NotEmpty();
    }
}
