using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Domain.FoodItems;
public sealed class FoodItem : Entity
{
    public FoodItem(
        Guid id,
        Guid foodReferenceId, 
        Guid compartmentId,
        Guid householdId,
        string name,
        Uri? imageUrl,
        decimal quantity, 
        Guid unitId, 
        DateOnly expirationDate,
        FoodItemStatus status,
        string? notes,
        Guid addedBy)
    {
        Id = id;
        FoodReferenceId = foodReferenceId;
        CompartmentId = compartmentId;
        HouseholdId = householdId;
        Name = name;
        ImageUrl = imageUrl;
        Quantity = quantity;
        UnitId = unitId;
        ExpirationDate = expirationDate;
        Status = status;
        Notes = notes;
        AddedBy = addedBy;
    }
    public FoodItem() { }
    public Guid FoodReferenceId { get; private set; }
    public Guid CompartmentId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public string Name { get; private set; }
    public Uri? ImageUrl { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public DateOnly ExpirationDate { get; private set; }
    public FoodItemStatus Status { get; private set; }
    public string? Notes { get; private set; } 
    public Guid AddedBy { get; private set; }
    public Guid? LastModifiedBy { get; private set; }

    public static FoodItem Create(
        Guid foodReferenceId,
        Guid compartmentId,
        Guid householdId,
        string name,
        Uri? imageUrl,
        decimal quantity,
        Guid unitId,
        DateOnly expirationDate,
        FoodItemStatus status,
        string? notes,
        Guid addedBy)
    {
        var foodItem = new FoodItem(
            Guid.CreateVersion7(),
            foodReferenceId,
            compartmentId,
            householdId,
            name,
            imageUrl,
            quantity,
            unitId,
            expirationDate,
            status,
            notes,
            addedBy);
        foodItem.Raise(new FoodItemAddedDomainEvent(foodItem.Id, quantity, unitId, addedBy));
        return foodItem;
    }

    public void AdjustQuantity(decimal newQuantity, Guid userId)
    {
        Raise(new FoodItemQuantityAdjustedDomainEvent(Id, newQuantity - Quantity, UnitId, userId));
        Quantity = newQuantity;
        LastModifiedBy = userId;
        
    }
    public void Consume(decimal qtyInItemUnit, decimal qtyInRequestUnit, Guid unitId, Guid userId)
    {
        Quantity -= qtyInItemUnit;
        LastModifiedBy = userId;
        Raise(new FoodItemConsumedDomainEvent(Id, qtyInRequestUnit, unitId, userId));
    }
    public void Discard(decimal qtyInItemUnit, decimal qtyInRequestUnit, Guid unitId, Guid userId)
    {
        Quantity -= qtyInItemUnit;
        LastModifiedBy = userId;
        Raise(new FoodItemDiscardedDomainEvent(Id, qtyInRequestUnit, unitId, userId));
    }
    public void Reserve(decimal qtyInItemUnit, decimal qtyInRequestUnit, Guid unitId, Guid userId)
    {
        Quantity -= qtyInItemUnit;
        LastModifiedBy = userId;
        Raise(new FoodItemReservedDomainEvent(Id, qtyInRequestUnit, unitId, userId));
    }
    public void CancelReservation(decimal quantity, Guid reservationId)
    {
        Quantity += quantity;
        Raise(new FoodItemReservationCancelledDomainEvent(reservationId));
    }
    public void ChangeUnit(Guid newUnitId, decimal convertedQuantity, Guid userId)
    {
        UnitId = newUnitId;
        Quantity = convertedQuantity;
        LastModifiedBy = userId;
    }
    public void AdjustExpirationDate(DateOnly newExpirationDate, Guid userId)
    {
        ExpirationDate = newExpirationDate;
        LastModifiedBy = userId;
    }
    public void UpdateNotes(string? newNotes, Guid userId)
    {
        Notes = newNotes;
        LastModifiedBy = userId;
    }
    public void UpdateImageUrl(Uri? newImageUrl, Guid userId)
    {
        ImageUrl = newImageUrl;
        LastModifiedBy = userId;
    }
    public void Rename(string newName, Guid userId)
    {
        Name = newName;
        LastModifiedBy = userId;
    }
    public void MoveToCompartment(Guid newCompartmentId, Guid userId)
    {
        CompartmentId = newCompartmentId;
        LastModifiedBy = userId;
    }

}
