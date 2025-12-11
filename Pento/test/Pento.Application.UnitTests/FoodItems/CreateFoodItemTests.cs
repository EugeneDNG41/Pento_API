using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.FoodItems.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Application.UnitTests.FoodItems;
internal sealed class CreateFoodItemTests
{
    private static readonly CreateFoodItemCommand command = new(
        FoodReferenceId: Guid.NewGuid(),
        CompartmentId: Guid.NewGuid(),
        Name: "Milk",
        Quantity: 2.0m,
        UnitId: Guid.NewGuid(),
        ExpirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
        Notes: "Organic whole milk"
    );
    private readonly CreateFoodItemCommandHandler _handler;
    private readonly IUserContext _userContextMock;
    private readonly IDateTimeProvider _dateTimeProviderMock;
    private readonly IGenericRepository<FoodReference> _foodReferenceRepositoryMock;
    private readonly IGenericRepository<Unit> _unitRepositoryMock;
    private readonly IGenericRepository<Compartment> _compartmentRepositoryMock;
    private readonly IGenericRepository<Storage> _storageRepositoryMock;
    private readonly IGenericRepository<FoodItem> _foodItemRepositoryMock;
    private readonly IHubContext<MessageHub, IMessageClient> _hubContextMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    public CreateFoodItemTests()
    {
        _userContextMock = Substitute.For<IUserContext>();
        _dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        _dateTimeProviderMock.UtcNow.Returns(DateTime.UtcNow);
        _foodReferenceRepositoryMock = Substitute.For<IGenericRepository<FoodReference>>();
        _unitRepositoryMock = Substitute.For<IGenericRepository<Unit>>();
        _compartmentRepositoryMock = Substitute.For<IGenericRepository<Compartment>>();
        _storageRepositoryMock = Substitute.For<IGenericRepository<Storage>>();
        _foodItemRepositoryMock = Substitute.For<IGenericRepository<FoodItem>>();
        _hubContextMock = Substitute.For<IHubContext<MessageHub, IMessageClient>>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _handler = new CreateFoodItemCommandHandler(
            _userContextMock,
            _dateTimeProviderMock,
            _foodReferenceRepositoryMock,
            _unitRepositoryMock,
            _compartmentRepositoryMock,
            _storageRepositoryMock,
            _foodItemRepositoryMock,
            _hubContextMock,
            _unitOfWorkMock);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenHouseholdIdIsNull()
    {
        // Arrange
        _userContextMock.HouseholdId.Returns((Guid?)null);
        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.That(result.IsFailure);
        Assert.That(result.Error == HouseholdErrors.NotInAnyHouseHold);
    }
    [Test]
    public async Task Handle_ShouldReturnError_WhenFoodReferenceNotFound()
    {
        // Arrange
        _userContextMock.HouseholdId.Returns(Guid.NewGuid());
        _foodReferenceRepositoryMock.GetByIdAsync(command.FoodReferenceId, Arg.Any<CancellationToken>())
            .Returns((FoodReference?)null);
        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.That(result.IsFailure);
        Assert.That(result.Error == FoodReferenceErrors.NotFound);
    }
    [Test]
    public async Task Handle_ShouldReturnError_WhenUnitNotFound()
    {
        // Arrange
        _userContextMock.HouseholdId.Returns(Guid.NewGuid());
        var foodReference = new FoodReference(
            id: command.FoodReferenceId,
            foodCategoryId: null,
            usdaId: "123456",
            foodGroup: FoodGroup.Dairy,
            barcode: null,
            typicalShelfLifeDaysFreezer: 30,
            typicalShelfLifeDaysFridge: 7,
            typicalShelfLifeDaysPantry: 0,
            addedBy: null,
            imageUrl: null,
            createdOnUtc: DateTime.UtcNow,
            name: "Milk",
            brand: "BrandA",
            unitType: UnitType.Volume);
        _foodReferenceRepositoryMock.GetByIdAsync(command.FoodReferenceId, Arg.Any<CancellationToken>())
            .Returns(foodReference);
        _unitRepositoryMock.GetByIdAsync(command.UnitId!.Value, Arg.Any<CancellationToken>())
            .Returns((Unit?)null);
        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.That(result.IsFailure);
        Assert.That(result.Error == UnitErrors.NotFound);
    }
    [Test]
    public async Task Handle_ShouldReturnError_WhenUnitTypeMismatch()
    {
        // Arrange
        _userContextMock.HouseholdId.Returns(Guid.NewGuid());
        var foodReference = new FoodReference(
            id: command.FoodReferenceId,
            foodCategoryId: null,
            usdaId: "123456",
            foodGroup: FoodGroup.Dairy,
            barcode: null,
            typicalShelfLifeDaysFreezer: 30,
            typicalShelfLifeDaysFridge: 7,
            typicalShelfLifeDaysPantry: 0,
            addedBy: null,
            imageUrl: null,
            createdOnUtc: DateTime.UtcNow,
            name: "Milk",
            brand: "BrandA",
            unitType: UnitType.Volume);
        _foodReferenceRepositoryMock.GetByIdAsync(command.FoodReferenceId, Arg.Any<CancellationToken>())
            .Returns(foodReference);
        var unit = new Unit(
            id: command.UnitId!.Value,
            name: "Kilogram",
            abbreviation: "kg",
            type: UnitType.Weight,
            toBaseFactor: 1.0m);
        _unitRepositoryMock.GetByIdAsync(command.UnitId!.Value, Arg.Any<CancellationToken>())
            .Returns(unit);
        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.That(result.IsFailure);
        Assert.That(result.Error == FoodItemErrors.InvalidMeasurementUnit);
    }
    [Test]
    public async Task Handle_ShouldCreateFoodItem_WhenCommandIsValid()
    {
        // Arrange
        var householdId = Guid.NewGuid();
        var storageId = Guid.NewGuid();
        _userContextMock.HouseholdId.Returns(householdId);
        var foodReference = new FoodReference(
            id: command.FoodReferenceId,
            foodCategoryId: null,
            usdaId: "123456",
            foodGroup: FoodGroup.Dairy,
            barcode: null,
            typicalShelfLifeDaysFreezer: 30,
            typicalShelfLifeDaysFridge: 7,
            typicalShelfLifeDaysPantry: 0,
            addedBy: null,
            imageUrl: null,
            createdOnUtc: DateTime.UtcNow,
            name: "Milk",
            brand: "BrandA",
            unitType: UnitType.Volume);
        _foodReferenceRepositoryMock.GetByIdAsync(command.FoodReferenceId, Arg.Any<CancellationToken>())
            .Returns(foodReference);
        var unit = new Unit(
            id: command.UnitId!.Value,
            name: "Liter",
            abbreviation: "L",
            type: UnitType.Volume,
            toBaseFactor: 1.0m);
        _unitRepositoryMock.GetByIdAsync(command.UnitId!.Value, Arg.Any<CancellationToken>())
            .Returns(unit);
        var compartment = new Compartment(
            id: command.CompartmentId,
            householdId: householdId,
            storageId: storageId,
            name: "Fridge Compartment",
            notes: null);
        _compartmentRepositoryMock.GetByIdAsync(command.CompartmentId, Arg.Any<CancellationToken>())
            .Returns(compartment);
        var storage = new Storage(
            id: storageId,
            householdId: householdId,
            name: "Main Fridge",
            type: StorageType.Fridge,
            notes: null);
        _storageRepositoryMock.GetByIdAsync(storageId, Arg.Any<CancellationToken>())
            .Returns(storage);
        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.That(result.IsSuccess);
        _foodItemRepositoryMock.Received(1).Add(Arg.Is<FoodItem>(fi =>
            fi.Name == command.Name &&
            fi.Quantity == command.Quantity &&
            fi.UnitId == command.UnitId &&
            fi.CompartmentId == command.CompartmentId &&
            fi.ExpirationDate == command.ExpirationDate &&
            fi.Notes == command.Notes &&
            fi.FoodReferenceId == command.FoodReferenceId &&
            fi.HouseholdId == householdId
        ));
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

}
