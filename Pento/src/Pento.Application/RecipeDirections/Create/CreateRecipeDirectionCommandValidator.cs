using FluentValidation;

namespace Pento.Application.RecipeDirections.Create;

internal sealed class CreateRecipeDirectionCommandValidator
    : AbstractValidator<CreateRecipeDirectionCommand>
{
    public CreateRecipeDirectionCommandValidator()
    {
        RuleFor(x => x.RecipeId)
            .NotEmpty()
            .WithMessage("RecipeId is required.");

        RuleFor(x => x.StepNumber)
            .GreaterThan(0)
            .WithMessage("StepNumber must be greater than zero.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ImageUrl)
            .Must(uri => uri == null || uri.IsAbsoluteUri)
            .WithMessage("ImageUrl must be a valid absolute URI if provided.");
    }
}
