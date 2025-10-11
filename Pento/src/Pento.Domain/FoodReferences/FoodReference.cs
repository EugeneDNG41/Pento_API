using System;
using System.Collections.Generic;
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
        string? barcode,
        string? brand,
        int typicalShelfLifeDays,
        string? openFoodFactsId,
        string? usdaId,
        DateTime createdOnUtc)
        : base(id)
    {
        Name = name;
        FoodGroup = foodGroup;
        Barcode = barcode;
        Brand = brand;
        TypicalShelfLifeDays = typicalShelfLifeDays;
        OpenFoodFactsId = openFoodFactsId;
        UsdaId = usdaId;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }
    public string Name { get; private set; }
    public FoodGroup FoodGroup { get; private set; }
    public string? Barcode { get; private set; }
    public string? Brand { get; private set; }
    public int TypicalShelfLifeDays { get; private set; }
    public string? OpenFoodFactsId { get; private set; }
    public string? UsdaId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }
    public static FoodReference Create(
           string name,
           FoodGroup foodGroup,
           string? barcode,
           string? brand,
           int typicalShelfLifeDays,
           string? openFoodFactsId,
           string? usdaId,
           DateTime utcNow)
    {
        var id = Guid.CreateVersion7();

        var foodRef = new FoodReference
        {
            Id = id,
            Name = name,
            FoodGroup = foodGroup,
            Barcode = barcode,
            Brand = brand,
            TypicalShelfLifeDays = typicalShelfLifeDays,
            OpenFoodFactsId = openFoodFactsId,
            UsdaId = usdaId,
            CreatedOnUtc = utcNow,
            UpdatedOnUtc = utcNow
        };

        foodRef.Raise(new FoodReferenceCreatedDomainEvent(foodRef.Id));

        return foodRef;
    }
    public void Update(
        string name,
        FoodGroup foodGroup,
        string? barcode,
        string? brand,
        int typicalShelfLifeDays,
        string? openFoodFactsId,
        string? usdaId,
        DateTime utcNow)
    {
        Name = name;
        FoodGroup = foodGroup;
        Barcode = barcode;
        Brand = brand;
        TypicalShelfLifeDays = typicalShelfLifeDays;
        OpenFoodFactsId = openFoodFactsId;
        UsdaId = usdaId;
        UpdatedOnUtc = utcNow;

        Raise(new FoodReferenceUpdatedDomainEvent(Id));
    }
}

