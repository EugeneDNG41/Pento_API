using NUnit.Framework;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.FoodReferences.Events;
using Pento.Domain.Units;

namespace Pento.Domain.UnitTests;

internal sealed class FoodReferenceTests
{
    /// <summary>
    /// Verifies that Create initializes all fields correctly,
    /// sets CreatedOnUtc and UpdatedOnUtc to utcNow,
    /// and raises exactly one FoodReferenceCreatedDomainEvent.
    /// </summary>
    [TestCaseSource(nameof(CreateValidInputs))]
    public void Create_ValidInputs_AssignsPropertiesAndRaisesEvent(
        string name,
        FoodGroup group,
        int? categoryId,
        string? brand,
        string? barcode,
        string usdaId,
        int? pantryDays,
        int? fridgeDays,
        int? freezerDays,
        Guid? addedBy,
        Uri? imageUrl,
        UnitType unitType,
        DateTime utcNow)
    {
        // Act
        var entity = FoodReference.Create(
            name,
            group,
            categoryId,
            brand,
            barcode,
            usdaId,
            pantryDays,
            fridgeDays,
            freezerDays,
            addedBy,
            imageUrl,
            unitType,
            utcNow);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entity.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(entity.Name, Is.EqualTo(name));
            Assert.That(entity.FoodGroup, Is.EqualTo(group));
            Assert.That(entity.FoodCategoryId, Is.EqualTo(categoryId));
            Assert.That(entity.Brand, Is.EqualTo(brand));
            Assert.That(entity.Barcode, Is.EqualTo(barcode));
            Assert.That(entity.UsdaId, Is.EqualTo(usdaId));
            Assert.That(entity.TypicalShelfLifeDays_Pantry, Is.EqualTo(pantryDays ?? 0));
            Assert.That(entity.TypicalShelfLifeDays_Fridge, Is.EqualTo(fridgeDays ?? 0));
            Assert.That(entity.TypicalShelfLifeDays_Freezer, Is.EqualTo(freezerDays ?? 0));
            Assert.That(entity.AddedBy, Is.EqualTo(addedBy));
            Assert.That(entity.ImageUrl, Is.EqualTo(imageUrl));
            Assert.That(entity.UnitType, Is.EqualTo(unitType));
            Assert.That(entity.CreatedOnUtc, Is.EqualTo(utcNow));
            Assert.That(entity.UpdatedOnUtc, Is.EqualTo(utcNow));

            IReadOnlyList<IDomainEvent> events = entity.GetDomainEvents();
            Assert.That(events.Count, Is.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<FoodReferenceCreatedDomainEvent>());
            Assert.That(((FoodReferenceCreatedDomainEvent)events[0]).FoodReferenceId, Is.EqualTo(entity.Id));
        });
    }

    /// <summary>
    /// Provides diverse valid inputs for Create().
    /// All FoodGroup values match the actual enum.
    /// </summary>
    private static IEnumerable<TestCaseData> CreateValidInputs()
    {
        yield return new TestCaseData(
            "Apple", FoodGroup.FruitsVegetables, null, null, null, "USDA_001",
            10, 5, 30, null, null, UnitType.Weight, DateTime.UtcNow
        ).SetName("Create_NormalValues");

        yield return new TestCaseData(
            "Milk", FoodGroup.Dairy, 123, "Vinamilk", "893123", "USDA_002",
            null, null, null, Guid.NewGuid(), new Uri("https://example.com/milk.png"),
            UnitType.Volume, new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        ).SetName("Create_NullShelfLifeAndImage");

        yield return new TestCaseData(
            new string('A', 1000), FoodGroup.CerealGrainsPasta, 999,    
            new string('B', 500), new string('C', 500), new string('D', 100),
            1, 2, 3, null, null, UnitType.Count, DateTime.MaxValue
        ).SetName("Create_LongValues_MaxDate");

        yield return new TestCaseData(
            "Sp€ciål Nämé", FoodGroup.Confectionery, null,
            "Bränd", "BAR\nCODE", "USDA_\u2603",
            7, 14, 90, null, new Uri("https://example.com/snowman.png"),
            UnitType.Weight, new DateTime(2005, 5, 5, 5, 5, 5, DateTimeKind.Utc)
        ).SetName("Create_SpecialCharacters");
    }

    /// <summary>
    /// Verifies that Update overwrites all fields and raises FoodReferenceUpdatedDomainEvent.
    /// </summary>
    [Test]
    public void Update_ValidInputs_UpdatesPropertiesAndRaisesEvent()
    {
        // Arrange
        var entity = FoodReference.Create(
            "OldName", FoodGroup.Meat, null, null, null, "USDA_X",
            1, 2, 3, null, null, UnitType.Count, DateTime.UtcNow);

        DateTime newUtc = DateTime.UtcNow.AddHours(1);
        var newUserId = Guid.NewGuid();
        var newImg = new Uri("https://img/new.png");

        // Act
        entity.Update(
            "NewName", FoodGroup.Seafood, 22, "BrandX", "123", "USDA_NEW",
            5, 10, 50, newUserId, newImg,
            UnitType.Weight, newUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entity.Name, Is.EqualTo("NewName"));
            Assert.That(entity.FoodGroup, Is.EqualTo(FoodGroup.Seafood));
            Assert.That(entity.FoodCategoryId, Is.EqualTo(22));
            Assert.That(entity.Brand, Is.EqualTo("BrandX"));
            Assert.That(entity.Barcode, Is.EqualTo("123"));
            Assert.That(entity.UsdaId, Is.EqualTo("USDA_NEW"));
            Assert.That(entity.TypicalShelfLifeDays_Pantry, Is.EqualTo(5));
            Assert.That(entity.TypicalShelfLifeDays_Fridge, Is.EqualTo(10));
            Assert.That(entity.TypicalShelfLifeDays_Freezer, Is.EqualTo(50));
            Assert.That(entity.AddedBy, Is.EqualTo(newUserId));
            Assert.That(entity.ImageUrl, Is.EqualTo(newImg));
            Assert.That(entity.UnitType, Is.EqualTo(UnitType.Weight));
            Assert.That(entity.UpdatedOnUtc, Is.EqualTo(newUtc));

            Assert.That(entity.GetDomainEvents().OfType<FoodReferenceUpdatedDomainEvent>().Count(), Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Verifies UpdateImageUrl updates the URL and raises event.
    /// </summary>
    [Test]
    public void UpdateImageUrl_ValidUrl_UpdatesAndRaisesEvent()
    {
        // Arrange
        var entity = FoodReference.Create(
            "Test", FoodGroup.Beverages, null, null, null, "USDA",
            1, 1, 1, null, null, UnitType.Count, DateTime.UtcNow);

        DateTime newUtc = DateTime.UtcNow.AddMinutes(30);
        var newUrl = new Uri("https://example.com/newimg.png");

        // Act
        entity.UpdateImageUrl(newUrl, newUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entity.ImageUrl, Is.EqualTo(newUrl));
            Assert.That(entity.UpdatedOnUtc, Is.EqualTo(newUtc));
            Assert.That(entity.GetDomainEvents().OfType<FoodReferenceUpdatedDomainEvent>().Count(), Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Verifies UpdateShelfLifeDays updates all values and raises event.
    /// </summary>
    [Test]
    public void UpdateShelfLifeDays_ValidInputs_UpdatesAndRaisesEvent()
    {
        // Arrange
        var entity = FoodReference.Create(
            "Test", FoodGroup.Condiments, null, null, null, "USDA",
            1, 1, 1, null, null, UnitType.Count, DateTime.UtcNow);

        DateTime newTime = DateTime.UtcNow.AddMinutes(10);

        // Act
        entity.UpdateShelfLifeDays(10, 20, 30, newTime);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entity.TypicalShelfLifeDays_Pantry, Is.EqualTo(10));
            Assert.That(entity.TypicalShelfLifeDays_Fridge, Is.EqualTo(20));
            Assert.That(entity.TypicalShelfLifeDays_Freezer, Is.EqualTo(30));
            Assert.That(entity.UpdatedOnUtc, Is.EqualTo(newTime));
            Assert.That(entity.GetDomainEvents().OfType<FoodReferenceUpdatedDomainEvent>().Count(), Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Verifies UpdateUnitType modifies value and raises event.
    /// </summary>
    [Test]
    public void UpdateUnitType_ValidInput_UpdatesAndRaisesEvent()
    {
        // Arrange
        var entity = FoodReference.Create(
            "Test", FoodGroup.MixedDishes, null, null, null, "USDA",
            1, 1, 1, null, null, UnitType.Count, DateTime.UtcNow);

        DateTime newTime = DateTime.UtcNow.AddMinutes(5);

        // Act
        entity.UpdateUnitType(UnitType.Volume, newTime);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entity.UnitType, Is.EqualTo(UnitType.Volume));
            Assert.That(entity.UpdatedOnUtc, Is.EqualTo(newTime));
            Assert.That(entity.GetDomainEvents().OfType<FoodReferenceUpdatedDomainEvent>().Count(), Is.EqualTo(1));
        });
    }
}
