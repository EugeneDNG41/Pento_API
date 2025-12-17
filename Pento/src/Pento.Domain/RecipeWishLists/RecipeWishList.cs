using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeWishLists;

public sealed class RecipeWishList : Entity
{
    public Guid HouseholdId { get; private set; }
    public Guid RecipeId { get; private set; }

    public DateTime AddedOnUtc { get; private set; }

    private RecipeWishList() { }

    private RecipeWishList(Guid householdId, Guid recipeId)
    {
        Id = Guid.CreateVersion7();
        HouseholdId = householdId;
        RecipeId = recipeId;
        AddedOnUtc = DateTime.UtcNow;
    }

    public static RecipeWishList Create(Guid householdId, Guid recipeId)
        => new(householdId, recipeId);


}
