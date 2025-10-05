using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

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

}
