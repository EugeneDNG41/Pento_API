using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.RecipeMedia;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Users;

namespace Pento.Domain.FoodItems;
public sealed class FoodItem : Entity
{
    public FoodItem(
        Guid id,
        Guid foodRefId, 
        Guid compartmentId,
        Guid householdId,
        string? customName, 
        decimal quantity, 
        Guid unitId, 
        DateTime expirationDateUtc, 
        string? notes,
        Guid? sourceItemId) : base(id)
    {
        FoodRefId = foodRefId;
        CompartmentId = compartmentId;
        HouseholdId = householdId;
        CustomName = customName;
        Quantity = quantity;
        UnitId = unitId;
        ExpirationDateUtc = expirationDateUtc;
        Notes = notes;
        SourceItemId = sourceItemId;
    }
    private FoodItem() { }
    public Guid FoodRefId { get; private set; }
    public Guid CompartmentId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public string? CustomName { get; private set; }
    public decimal Quantity { get; private set; }
    public Guid UnitId { get; private set; }
    public DateTime ExpirationDateUtc { get; private set; }
    public string? Notes { get; private set; } 
    public Guid? SourceItemId { get; private set; } // If created from split
    public static FoodItem Create(
        Guid foodRefId,
        Guid compartmentId,
        Guid householdId,
        string? customName,
        decimal quantity,
        Guid unitId,
        DateTime expirationDateUtc,
        string? notes,
        Guid? sourceItemId)
    {
        var foodItem = new FoodItem();
        foodItem.Apply(new FoodItemAdded(
            Guid.CreateVersion7(),
            foodRefId,
            compartmentId,
            householdId,
            customName,
            quantity,
            unitId,
            expirationDateUtc,
            notes,
            sourceItemId));
        return foodItem;
    }
    public void Apply(FoodItemAdded @event) 
    {
        FoodRefId = @event.FoodRefId;
        CompartmentId = @event.CompartmentId;
        HouseholdId = @event.HouseholdId;
        CustomName = @event.CustomName;
        Quantity = @event.Quantity;
        UnitId = @event.UnitId;
        ExpirationDateUtc = @event.ExpirationDateUtc;
        Notes = @event.Notes;
    }

    public void Rename(string? newName)
    {
        CustomName = newName;
    }
    public void AdjustQuantity(decimal newQuantity)
    {
        Quantity = newQuantity;
    }
    public void ChangeExpirationDate(DateTime newExpirationDateUtc)
    {
        ExpirationDateUtc = newExpirationDateUtc;
    }
    public void UpdateNotes(string? newNotes)
    {
        Notes = newNotes;
    }
    public void ChangeMeasurementUnit(Guid newUnitId, decimal newQuantity)
    {
        UnitId = newUnitId;
        Quantity = newQuantity;
    }
    public void Move(Guid newCompartmentId)
    {
        CompartmentId = newCompartmentId;
    }
}
