using FluentValidation;

namespace Pento.Application.FoodItems.UpdateNotes;

internal sealed class UpdateFoodItemNotesCommandValidator : AbstractValidator<UpdateFoodItemNotesCommand>
{
    public UpdateFoodItemNotesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.NewNotes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
    }
}
