using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences.Events;

namespace Pento.Domain.FoodReferences;

public sealed class FoodReference : Entity
{
    private FoodReference() { }

    public FoodReference(
      Guid id,
        string name,
        FoodGroup foodGroup,
        FoodDataType dataType,
        string? notes,
        int? foodCategoryId,
        string? brand,
        string? barcode,
        string usdaId,
        DateTime publishedOnUtc,
        int? typicalShelfLifeDaysPantry,
        int? typicalShelfLifeDaysFridge,
        int? typicalShelfLifeDaysFreezer,
        Guid? addedBy,
        Uri? imageUrl,
        DateTime createdOnUtc)
        : base(id)
    {
        Name = name;
        FoodGroup = foodGroup;
        DataType = dataType;
        Notes = notes;
        FoodCategoryId = foodCategoryId;
        Brand = brand;
        Barcode = barcode;
        UsdaId = usdaId;
        PublishedOnUtc = publishedOnUtc;
        TypicalShelfLifeDays_Pantry = typicalShelfLifeDaysPantry ?? 0;
        TypicalShelfLifeDays_Fridge = typicalShelfLifeDaysFridge ?? 0;
        TypicalShelfLifeDays_Freezer = typicalShelfLifeDaysFreezer ?? 0;
        AddedBy= addedBy;
        ImageUrl = imageUrl;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }
    public string Name { get; private set; }
    public FoodGroup FoodGroup { get; private set; }
    public FoodDataType DataType { get; private set; } // "foundation_food"
    public string? Notes { get; private set; }
    public int? FoodCategoryId { get; private set; }
    public string? Brand { get; private set; }
    public string? Barcode { get; private set; }
    public string UsdaId { get; private set; }
    public DateTime PublishedOnUtc { get; private set; }
    public int TypicalShelfLifeDays_Pantry { get; private set; }
    public int TypicalShelfLifeDays_Fridge { get; private set; }
    public int TypicalShelfLifeDays_Freezer { get; private set; }
    public Guid? AddedBy { get; private set; }

    public Uri? ImageUrl { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }
    public static FoodReference Create(
        string name,
        FoodGroup foodGroup,
        FoodDataType dataType,
        string? notes,
        int? foodCategoryId,
        string? brand,
        string? barcode,
        string usdaId,
        DateTime publishedOnUtc,
        int? typicalShelfLifeDaysPantry,
        int? typicalShelfLifeDaysFridge,
        int? typicalShelfLifeDaysFreezer,
        Guid? addedBy,
        Uri? imageUrl,
        DateTime utcNow)
    {
        var entity = new FoodReference(
            Guid.CreateVersion7(),
            name,
            foodGroup,
            dataType,
            notes,
            foodCategoryId,
            brand,
            barcode,
            usdaId,
            publishedOnUtc,
            typicalShelfLifeDaysPantry,
            typicalShelfLifeDaysFridge,
            typicalShelfLifeDaysFreezer,
            addedBy,
            imageUrl,
            utcNow
        );

        entity.Raise(new FoodReferenceCreatedDomainEvent(entity.Id));
        return entity;
    }
    public void Update(
        string name,
        FoodGroup foodGroup,
        FoodDataType dataType,
        string? notes,
        int? foodCategoryId,
        string? brand,
        string? barcode,
        string usdaId,
        DateTime publishedOnUtc,
        int? typicalShelfLifeDaysPantry,
        int? typicalShelfLifeDaysFridge,
        int? typicalShelfLifeDaysFreezer,
        Guid? addedBy,
        Uri? imageUrl,
        DateTime utcNow)
    {
        Name = name;
        FoodGroup = foodGroup;
        DataType = dataType;
        Notes = notes;
        FoodCategoryId = foodCategoryId;
        Brand = brand;
        Barcode = barcode;
        UsdaId = usdaId;
        PublishedOnUtc = publishedOnUtc;
        TypicalShelfLifeDays_Pantry = typicalShelfLifeDaysPantry ?? 0;
        TypicalShelfLifeDays_Fridge = typicalShelfLifeDaysFridge ?? 0;
        TypicalShelfLifeDays_Freezer = typicalShelfLifeDaysFreezer ?? 0;
        AddedBy = addedBy;
        ImageUrl = imageUrl;
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

