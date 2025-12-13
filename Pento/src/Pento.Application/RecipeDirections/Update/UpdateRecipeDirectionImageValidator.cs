using FluentValidation;

namespace Pento.Application.RecipeDirections.Update;

internal sealed class UpdateRecipeDirectionImageValidator
    : AbstractValidator<UpdateRecipeDirectionImageCommand>
{
    private static readonly string[] AllowedTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private const long MaxSize = 5 * 1024 * 1024;

    public UpdateRecipeDirectionImageValidator()
    {
        RuleFor(x => x.RecipeDirectionId)
            .NotEmpty().WithMessage("Recipe Direcion ID is required.");


        RuleFor(x => x.ImageFile)
            .NotNull().WithMessage("Image file is required.")
            .Must(f => f.Length > 0).WithMessage("Image file cannot be empty.")
            .Must(f => f.Length <= MaxSize)
            .WithMessage($"Image must be <= {MaxSize / (1024 * 1024)}MB.")
            .Must(f => AllowedTypes.Contains(f.ContentType))
            .WithMessage("Only JPEG, PNG, WebP, GIF formats are allowed.");
    }
}
