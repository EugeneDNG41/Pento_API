using Pento.Domain.Abstractions;
using Pento.Domain.RecipeIngredients.Events;

namespace Pento.Domain.RecipeIngredients;

public sealed class RecipeIngredient : Entity
{
    public RecipeIngredient(
       Guid id,
       Guid recipeId,
       Guid foodRefId,
       decimal quantity,
       Guid unitId,
       string? notes,
       DateTime createdOnUtc)
       : base(id)
    {

        RecipeId = recipeId;
        FoodRefId = foodRefId;
        Quantity = quantity;
        UnitId = unitId;
        Notes = notes;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    private RecipeIngredient() { }

    public Guid RecipeId { get; private set; }

    public Guid FoodRefId { get; private set; }

    public decimal Quantity { get; private set; }

    public Guid UnitId { get; private set; }

    public string? Notes { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }

    public static RecipeIngredient Create(
          Guid recipeId,
          Guid foodRefId,
          decimal quantity,
          Guid unitId,
          string? notes,
          DateTime utcNow)
    {
        var ingredient = new RecipeIngredient(
            Guid.NewGuid(),
            recipeId,
            foodRefId,
            quantity,
            unitId,
            notes,
            utcNow);


        return ingredient;
    }

    public void UpdateDetails(
        decimal? quantity,
        Guid? unitId,
        string? notes,
        DateTime utcNow)
    {
        if (quantity.HasValue && quantity.Value > 0)
        {
            Quantity = quantity.Value;
        }

        if (unitId.HasValue)
        {
            UnitId = unitId.Value;
        }

        Notes = notes;
        UpdatedOnUtc = utcNow;

    }

}
