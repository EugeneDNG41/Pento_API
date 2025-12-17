using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.FoodItems.Consume;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Units;

namespace Pento.Application.UnitTests.FoodItems;

internal sealed class ConsumeFoodItemTests
{
    private readonly IConverterService _converter;
    private readonly IUserContext _userContext;
    private readonly IGenericRepository<FoodItem> _foodItemRepo;
    private readonly IHubContext<MessageHub, IMessageClient> _hub;
    private readonly IMessageClient _client;
    private readonly IHubClients<IMessageClient> _clients;
    private readonly IUnitOfWork _uow;

    private readonly ConsumeFoodItemCommandHandler _handler;

    public ConsumeFoodItemTests()
    {
        _converter = Substitute.For<IConverterService>();
        _userContext = Substitute.For<IUserContext>();
        _foodItemRepo = Substitute.For<IGenericRepository<FoodItem>>();
        _hub = Substitute.For<IHubContext<MessageHub, IMessageClient>>();
        _clients = Substitute.For<IHubClients<IMessageClient>>();
        _client = Substitute.For<IMessageClient>();
        _uow = Substitute.For<IUnitOfWork>();

        _hub.Clients.Returns(_clients);
        _clients.Group(Arg.Any<string>()).Returns(_client);

        _handler = new ConsumeFoodItemCommandHandler(
            _converter,
            _userContext,
            _foodItemRepo,
            _hub,
            _uow);
    }

    private static ConsumeFoodItemCommand CreateCommand(
        Guid foodItemId,
        decimal quantity,
        Guid unitId)
        => new(foodItemId, quantity, unitId);

    private static FoodItem CreateFoodItem(Guid householdId, Guid unitId, decimal quantity)
        => FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            "Milk",
            null,
            quantity,
            unitId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            null,
            Guid.NewGuid());

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenFoodItemNotFound()
    {
        _foodItemRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((FoodItem?)null);

        Result result = await _handler.Handle(CreateCommand(Guid.NewGuid(), 1, Guid.NewGuid()), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.NotFound));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenHouseholdMismatch()
    {
        FoodItem foodItem = CreateFoodItem(Guid.NewGuid(), Guid.NewGuid(), 5);

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _userContext.HouseholdId.Returns(Guid.NewGuid());

        Result result = await _handler.Handle(CreateCommand(foodItem.Id, 1, foodItem.UnitId), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.ForbiddenAccess));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenQuantityExceedsAvailable()
    {
        var householdId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(householdId, unitId, 1);

        _userContext.HouseholdId.Returns(householdId);

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        Result result = await _handler.Handle(CreateCommand(foodItem.Id, 5, unitId), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.InsufficientQuantity));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenConversionFails()
    {
        var householdId = Guid.NewGuid();
        var foodItemUnit = Guid.NewGuid();
        var requestUnit = Guid.NewGuid();

        FoodItem foodItem = CreateFoodItem(householdId, foodItemUnit, 10);

        _userContext.HouseholdId.Returns(householdId);

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _converter.ConvertAsync(
            Arg.Any<decimal>(),
            requestUnit,
            foodItemUnit,
            Arg.Any<CancellationToken>())
            .Returns(Result.Failure<decimal>(UnitErrors.NotFound));

        Result result = await _handler.Handle(CreateCommand(foodItem.Id, 2, requestUnit), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(UnitErrors.NotFound));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ShouldConsumeFoodItem_WhenSameUnit()
    {
        var householdId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(householdId, unitId, 5);

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        Result result = await _handler.Handle(CreateCommand(foodItem.Id, 2, unitId), CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(foodItem.Quantity, Is.EqualTo(3));

        await _foodItemRepo.Received(1).UpdateAsync(foodItem);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // ---------- TEST 6 ----------
    [Test]
    public async Task Handle_ShouldConsumeFoodItem_WhenUnitIsConverted()
    {
        var householdId = Guid.NewGuid();
        var itemUnit = Guid.NewGuid();
        var requestUnit = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(householdId, itemUnit, 10);

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _converter.ConvertAsync(
            2,
            requestUnit,
            itemUnit,
            Arg.Any<CancellationToken>())
            .Returns(Result.Success(4m));

        Result result = await _handler.Handle(CreateCommand(foodItem.Id, 2, requestUnit), CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(foodItem.Quantity, Is.EqualTo(6));

        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // ---------- TEST 7 ----------
    [Test]
    public async Task Handle_ShouldNotifyHouseholdAfterConsume()
    {
        var householdId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(householdId, unitId, 5);

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        await _handler.Handle(CreateCommand(foodItem.Id, 1, unitId), CancellationToken.None);

        await _client.Received(1)
            .FoodItemQuantityUpdated(foodItem.Id, foodItem.Quantity);
    }
}
