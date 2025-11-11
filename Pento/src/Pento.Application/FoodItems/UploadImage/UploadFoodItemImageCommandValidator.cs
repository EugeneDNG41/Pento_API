using FluentValidation;

namespace Pento.Application.FoodItems.UploadImage;

internal sealed class UploadFoodItemImageCommandValidator : AbstractValidator<UploadFoodItemImageCommand>
{
    public UploadFoodItemImageCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Food item Id must not be empty.");
        RuleFor(x => x.File)
            .Must(file => file is null || file.Length > 0).WithMessage("File must not be empty if provided.");
    }
}
