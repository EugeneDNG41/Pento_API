using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems.Events;

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
            notes,
            addedBy);
        foodItem.Raise(new FoodItemAddedDomainEvent(foodItem.Id, quantity, unitId, addedBy));
        return foodItem;
    }

    public void AdjustQuantity(decimal newQuantity, Guid UserId)
    {
        Quantity = newQuantity;
        LastModifiedBy = UserId;
    }
    public void Consume(decimal quantity, Guid UserId)
    {
        Quantity -= quantity;
        LastModifiedBy = UserId;
        Raise(new FoodItemConsumedDomainEvent(Id, quantity, UnitId, UserId));
    }
    public void Discard(decimal quantity, Guid UserId)
    {
        Quantity -= quantity;
        LastModifiedBy = UserId;
        Raise(new FoodItemDiscardedDomainEvent(Id, quantity, UnitId, UserId));
    }
    public void Reserve(decimal quantity)
    {
        Quantity -= quantity;
        Raise(new FoodItemReservedDomainEvent(Id, quantity, UnitId));
    }
    public void CancelReservation(decimal quantity, Guid reservationId)
    {
        Quantity += quantity;
        Raise(new FoodItemReservationCancelledDomainEvent(reservationId));
    }
    public void ChangeUnit(Guid newUnitId, decimal convertedQuantity, Guid UserId)
    {
        UnitId = newUnitId;
        Quantity = convertedQuantity;
        LastModifiedBy = UserId;
    }
    public void AdjustExpirationDate(DateOnly newExpirationDate, Guid UserId)
    {
        ExpirationDate = newExpirationDate;
        LastModifiedBy = UserId;
    }
    public void UpdateNotes(string? newNotes, Guid UserId)
    {
        Notes = newNotes;
        LastModifiedBy = UserId;
    }
    public void UpdateImageUrl(Uri? newImageUrl, Guid UserId)
    {
        ImageUrl = newImageUrl;
        LastModifiedBy = UserId;
    }
    public void Rename(string newName, Guid UserId)
    {
        Name = newName;
        LastModifiedBy = UserId;
    }
    public void MoveToCompartment(Guid newCompartmentId, Guid UserId)
    {
        CompartmentId = newCompartmentId;
        LastModifiedBy = UserId;
    }

}
