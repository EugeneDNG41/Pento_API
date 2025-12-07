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
        Notes = notes;
        Priority = priority;
    }
    public void Update(
    decimal quantity,
    string? notes,
    string? customName,
    GroceryItemPriority priority)
    {

        Quantity = quantity;
        Notes = notes;
        CustomName = customName;
        Priority = priority;

    }
    private GroceryListItem() { }
    public void IncreaseQuantity(decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Quantity += amount;
    }


    public Guid ListId { get; private set; }
    public Guid FoodRefId { get; private set; }
    public string? CustomName { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid? UnitId { get; private set; }
    public string? Notes { get; private set; }
    public GroceryItemPriority Priority { get; private set; }
    public Guid AddedBy { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
}
