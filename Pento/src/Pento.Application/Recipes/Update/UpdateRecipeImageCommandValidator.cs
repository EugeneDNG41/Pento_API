using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Pento.Application.Recipes.Update;

public sealed class UpdateRecipeImageCommandValidator
    : AbstractValidator<UpdateRecipeImageCommand>
{
    private static readonly string[] AllowedImageTypes =
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UpdateRecipeImageCommandValidator()
    {
        RuleFor(x => x.RecipeId)
            .NotEmpty().WithMessage("Recipe ID is required.");

        RuleFor(x => x.ImageFile)
            .NotNull().WithMessage("Image file is required.")
            .Must(BeValidFile).WithMessage("Invalid image file.")
            .Must(f => f.Length > 0).WithMessage("Image file cannot be empty.")
            .Must(f => f.Length <= MaxFileSizeBytes)
                .WithMessage($"Image size must not exceed {MaxFileSizeBytes / (1024 * 1024)}MB.")
            .Must(f => AllowedImageTypes.Contains(f.ContentType))
                .WithMessage("Only JPEG, PNG, WebP, GIF formats are allowed.");
    }

    private static bool BeValidFile(IFormFile file)
    {
        return file is not null;
    }
}
