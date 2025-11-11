using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences.Events;
using Pento.Domain.Units;

namespace Pento.Domain.FoodReferences;

public sealed class FoodReference : Entity
{
    private FoodReference() { }

    public FoodReference(
      Guid id,
        string name,
        FoodGroup foodGroup,
        int? foodCategoryId,
        string? brand,
        string? barcode,
        string usdaId,
        int? typicalShelfLifeDaysPantry,
        int? typicalShelfLifeDaysFridge,
        int? typicalShelfLifeDaysFreezer,
        Guid? addedBy,
        Uri? imageUrl,
        UnitType unitType,
        DateTime createdOnUtc)
        : base(id)
    {
        Name = name;
        FoodGroup = foodGroup;
        FoodCategoryId = foodCategoryId;
        Brand = brand;
        Barcode = barcode;
        UsdaId = usdaId;
        TypicalShelfLifeDays_Pantry = typicalShelfLifeDaysPantry ?? 0;
        TypicalShelfLifeDays_Fridge = typicalShelfLifeDaysFridge ?? 0;
        TypicalShelfLifeDays_Freezer = typicalShelfLifeDaysFreezer ?? 0;
        AddedBy= addedBy;
        ImageUrl = imageUrl;
        UnitType = unitType;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }
    public string Name { get; private set; }
    public FoodGroup FoodGroup { get; private set; }
    public int? FoodCategoryId { get; private set; }
    public string? Brand { get; private set; }
    public string? Barcode { get; private set; }
    public string UsdaId { get; private set; }
    public int TypicalShelfLifeDays_Pantry { get; private set; }
    public int TypicalShelfLifeDays_Fridge { get; private set; }
    public int TypicalShelfLifeDays_Freezer { get; private set; }
    public Guid? AddedBy { get; private set; }
    public Uri? ImageUrl { get; private set; }
    public UnitType UnitType { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }
    public static FoodReference Create(
        string name,
        FoodGroup foodGroup,
        int? foodCategoryId,
        string? brand,
        string? barcode,
        string usdaId,
        int? typicalShelfLifeDaysPantry,
        int? typicalShelfLifeDaysFridge,
        int? typicalShelfLifeDaysFreezer,
        Guid? addedBy,
        Uri? imageUrl,
        UnitType unitType,
        DateTime utcNow)
    {
        var entity = new FoodReference(
            Guid.CreateVersion7(),
            name,
            foodGroup,
            foodCategoryId,
            brand,
            barcode,
            usdaId,
            typicalShelfLifeDaysPantry,
            typicalShelfLifeDaysFridge,
            typicalShelfLifeDaysFreezer,
            addedBy,
            imageUrl,
            unitType,
            utcNow
        );

        entity.Raise(new FoodReferenceCreatedDomainEvent(entity.Id));
        return entity;
    }
    public void Update(
        string name,
        FoodGroup foodGroup,
        int? foodCategoryId,
        string? brand,
        string? barcode,
        string usdaId,
        int? typicalShelfLifeDaysPantry,
        int? typicalShelfLifeDaysFridge,
        int? typicalShelfLifeDaysFreezer,
        Guid? addedBy,
        Uri? imageUrl,
        UnitType unitType,
        DateTime utcNow)
    {
        Name = name;
        FoodGroup = foodGroup;
        FoodCategoryId = foodCategoryId;
        Brand = brand;
        Barcode = barcode;
        UsdaId = usdaId;
        TypicalShelfLifeDays_Pantry = typicalShelfLifeDaysPantry ?? 0;
        TypicalShelfLifeDays_Fridge = typicalShelfLifeDaysFridge ?? 0;
        TypicalShelfLifeDays_Freezer = typicalShelfLifeDaysFreezer ?? 0;
        AddedBy = addedBy;
        ImageUrl = imageUrl;
        UnitType = unitType;
        UpdatedOnUtc = utcNow;

        Raise(new FoodReferenceUpdatedDomainEvent(Id));
    }
    public void UpdateImageUrl(Uri newUrl, DateTime utcNow)
    {
        ImageUrl = newUrl;
        UpdatedOnUtc = utcNow;

        Raise(new FoodReferenceUpdatedDomainEvent(Id));
    }
    public void UpdateShelfLifeDays(int pantryDays, int fridgeDays, int freezerDays, DateTime updatedAtUtc)
    {
        TypicalShelfLifeDays_Pantry = pantryDays;
        TypicalShelfLifeDays_Fridge = fridgeDays;
        TypicalShelfLifeDays_Freezer = freezerDays;
        UpdatedOnUtc = updatedAtUtc;
        Raise(new FoodReferenceUpdatedDomainEvent(Id));

    }

}

