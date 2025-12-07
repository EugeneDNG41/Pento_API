using FluentValidation;

namespace Pento.Application.FoodItems.Search;

internal sealed class SearchFoodItemQueryValidator : AbstractValidator<SearchFoodItemQuery>
{
    public SearchFoodItemQueryValidator()
    {
        RuleFor(x => x.FromQuantity).GreaterThanOrEqualTo(0).When(x => x.FromQuantity is not null)
            .WithMessage("From quantity must be greater than or equal to 0.");
        RuleFor(x => x.ToQuantity).GreaterThanOrEqualTo(0).When(x => x.ToQuantity is not null)
            .WithMessage("To quantity must be greater than or equal to 0.");
        RuleFor(x => x).Must(x => x.FromQuantity <= x.ToQuantity).When(x => x.FromQuantity is not null && x.ToQuantity is not null)
            .WithMessage("From quantity must be less than or equal to To quantity.");
        RuleFor(x => x).Must(x => x.ExpirationDateAfter <= x.ExpirationDateBefore).When(x => x.ExpirationDateAfter is not null && x.ExpirationDateBefore is not null)
            .WithMessage("Expiration date after must be less than or equal to Expiration date before.");
        RuleForEach(x => x.FoodGroup)
            .IsInEnum().When(x => x.FoodGroup is not null)
            .WithMessage("Invalid food group value.");
        RuleForEach(x => x.Status)
            .IsInEnum().When(x => x.Status is not null)
            .WithMessage("Invalid food item status value.");
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
        RuleFor(x => x.SortBy)
            .IsInEnum().WithMessage("Invalid sort by value.");
        RuleFor(x => x.SortOrder)
            .IsInEnum().WithMessage("Invalid sort order value.");

    }
}
