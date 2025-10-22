using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.StorageItems;

public sealed class StorageItem : Entity
{
    public StorageItem(
        Guid id,
        Guid foodRefId, 
        Guid storageId, 
        Guid compartmentId, 
        string? customName, 
        decimal quantity, 
        Guid unitId, 
        StorageItemStatus status, 
        DateTime expirationDateUtc, 
        string? notes, 
        DateTime createdOnUtc, 
        DateTime updatedOnUtc) : base(id)
    {
        FoodRefId = foodRefId;
        StorageId = storageId;
        CompartmentId = compartmentId;
        CustomName = customName;
        Quantity = quantity;
        UnitId = unitId;
        Status = status;
        ExpirationDateUtc = expirationDateUtc;
        Notes = notes;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = updatedOnUtc;
    }
    private StorageItem() { }
    public Guid FoodRefId { get; private set; }
    public Guid StorageId { get; private set; }
    public Guid CompartmentId { get; private set; }
    public string? CustomName { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public StorageItemStatus Status { get; private set; }
    public DateTime ExpirationDateUtc { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }
}
public enum StorageItemStatus
{
    Available,
    Reserved,
    Consumed,
    Donated,
    Disposed
}
