using FluentValidation;

namespace Pento.Application.FoodItems.Merge;

internal sealed class MergeFoodItemCommandValidator : AbstractValidator<MergeFoodItemCommand>
{
    public MergeFoodItemCommandValidator()
    {
        RuleFor(x => x.SourceId)
            .NotEmpty().WithMessage("Source food item Id must not be empty.");
        RuleFor(x => x.TargetId)
            .NotEmpty().WithMessage("Target food item Id must not be empty.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("BaseQuantity must be greater than zero.");
        RuleFor(x => x)
            .Must(x => x.SourceId != x.TargetId)
            .WithMessage("Source and target food item Ids must be different.");
    }
}
