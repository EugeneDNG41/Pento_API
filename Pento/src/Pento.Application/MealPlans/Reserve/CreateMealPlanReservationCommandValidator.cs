using FluentValidation;

namespace Pento.Application.MealPlans.Reserve;

internal sealed class CreateMealPlanReservationCommandValidator
    : AbstractValidator<CreateMealPlanReservationCommand>
{
    public CreateMealPlanReservationCommandValidator()
    {
        RuleFor(x => x.FoodItemId)
            .NotEmpty();


        RuleFor(x => x.UnitId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.MealType)
     .NotNull()
     .WithMessage("MealType is required.");

        RuleFor(x => x.ScheduledDate)
            .NotNull()
            .WithMessage("ScheduledDate is required.");
        RuleFor(x => x.ScheduledDate)
            .Must(d => d >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("ScheduledDate cannot be earlier than today.");
    }
}
