using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeWishLists;

public sealed class RecipeWishList : BaseEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid RecipeId { get; private set; }

    public DateTime AddedOnUtc { get; private set; }
    public Guid? HouseholdId { get; private set; }

    private RecipeWishList() { }

    private RecipeWishList(Guid userId, Guid recipeId, Guid? householdId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        RecipeId = recipeId;
        AddedOnUtc = DateTime.UtcNow;
        HouseholdId = householdId;
    }

    public static RecipeWishList Create(Guid userId, Guid recipeId, Guid? householdId)
        => new(userId, recipeId, householdId);


}
