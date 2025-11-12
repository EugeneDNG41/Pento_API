using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.GroceryListItems;
public sealed class GroceryListItem : Entity
{
    public GroceryListItem(
        Guid id,
        Guid listId,
        Guid foodRefId,
        decimal quantity,
        Guid addedBy,
        DateTime createdOnUtc,
        string? customName = null,
        Guid? unitId = null,
        decimal? estimatedPrice = null,
        string? notes = null,
        GroceryItemPriority priority = GroceryItemPriority.Medium
    ) : base(id)
    {
        ListId = listId;
        FoodRefId = foodRefId;
        Quantity = quantity;
        AddedBy = addedBy;
        CreatedOnUtc = createdOnUtc;
        CustomName = customName;
        UnitId = unitId;
        EstimatedPrice = estimatedPrice;
        Notes = notes;
        Priority = priority;
    }
    public void Update(
    decimal quantity,
    string? notes,
    string? customName,
    decimal? estimatedPrice,
    GroceryItemPriority priority)
    {

        Quantity = quantity;
        Notes = notes;
        CustomName = customName;
        EstimatedPrice = estimatedPrice;
        Priority = priority;

    }
    private GroceryListItem() { }

    public Guid ListId { get; private set; }
    public Guid FoodRefId { get; private set; }
    public string? CustomName { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid? UnitId { get; private set; }
    public decimal? EstimatedPrice { get; private set; }
    public string? Notes { get; private set; }
    public GroceryItemPriority Priority { get; private set; }
    public Guid AddedBy { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
}
