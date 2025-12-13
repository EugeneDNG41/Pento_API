using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.MealPlans.Reserve;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;

namespace Pento.Application.UnitTests.MealPlans;

internal sealed class CreateMealPlanReservationTests
{
    private readonly IGenericRepository<FoodItem> _foodItemRepo;
    private readonly IGenericRepository<MealPlan> _mealPlanRepo;
    private readonly IGenericRepository<FoodItemMealPlanReservation> _reservationRepo;
    private readonly IConverterService _converter;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _clock;
    private readonly IUnitOfWork _uow;

    private readonly CreateMealPlanReservationCommandHandler _handler;

    public CreateMealPlanReservationTests()
    {
        _foodItemRepo = Substitute.For<IGenericRepository<FoodItem>>();
        _mealPlanRepo = Substitute.For<IGenericRepository<MealPlan>>();
        _reservationRepo = Substitute.For<IGenericRepository<FoodItemMealPlanReservation>>();
        _converter = Substitute.For<IConverterService>();
        _userContext = Substitute.For<IUserContext>();
        _clock = Substitute.For<IDateTimeProvider>();
        _uow = Substitute.For<IUnitOfWork>();

        _clock.UtcNow.Returns(DateTime.UtcNow);

        _handler = new CreateMealPlanReservationCommandHandler(
            _foodItemRepo,
            _mealPlanRepo,
            _reservationRepo,
            _converter,
            _userContext,
            _clock,
            _uow);
    }

    private static CreateMealPlanReservationCommand CreateCommand(
        Guid foodItemId,
        Guid unitId,
        decimal qty)
        => new(
            FoodItemId: foodItemId,
            MealType: MealType.Dinner,
            ScheduledDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Servings: 2,
            Quantity: qty,
            UnitId: unitId
        );

    private static FoodItem CreateFoodItem(Guid householdId, Guid unitId, decimal qty)
        => FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            "Chicken",
            null,
            qty,
            unitId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            null,
            Guid.NewGuid());

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotInHousehold()
    {
        _userContext.HouseholdId.Returns((Guid?)null);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid(), Guid.NewGuid(), 1),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(HouseholdErrors.NotInAnyHouseHold));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenFoodItemNotFound()
    {
        _userContext.HouseholdId.Returns(Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((FoodItem?)null);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid(), Guid.NewGuid(), 1),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.NotFound));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenFoodItemNotInHousehold()
    {
        var householdId = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(Guid.NewGuid(), Guid.NewGuid(), 5);

        _userContext.HouseholdId.Returns(householdId);
        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id, foodItem.UnitId, 1),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.ForbiddenAccess));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldCreateMealPlan_WhenNotExists()
    {
        var householdId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(householdId, unitId, 5);

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _mealPlanRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<MealPlan, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id, unitId, 2),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        _mealPlanRepo.Received(1).Add(Arg.Any<MealPlan>());
        _reservationRepo.Received(1).Add(Arg.Any<FoodItemMealPlanReservation>());
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ShouldReuseExistingMealPlan()
    {
        var householdId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(householdId, unitId, 5);
        var mealPlan = MealPlan.Create(
            householdId,
            "Dinner",
            MealType.Dinner,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            2,
            null,
            Guid.NewGuid(),
            DateTime.UtcNow);

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _mealPlanRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<MealPlan, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([mealPlan]);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id, unitId, 1),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        _mealPlanRepo.DidNotReceive().Add(Arg.Any<MealPlan>());
        _reservationRepo.Received(1).Add(Arg.Any<FoodItemMealPlanReservation>());
    }

    // ---------- TEST 6 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenConvertedQuantityExceedsStock()
    {
        var householdId = Guid.NewGuid();
        var itemUnit = Guid.NewGuid();
        var requestUnit = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(householdId, itemUnit, 3);

        _userContext.HouseholdId.Returns(householdId);
        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _converter.ConvertAsync(Arg.Any<decimal>(), requestUnit, itemUnit, Arg.Any<CancellationToken>())
            .Returns(Result.Success(10m));

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id, requestUnit, 5),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.InsufficientQuantity));
    }

    // ---------- TEST 7 ----------
    [Test]
    public async Task Handle_HappyPath_ReservesFoodItemAndSaves()
    {
        var householdId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        FoodItem foodItem = CreateFoodItem(householdId, unitId, 5);

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _mealPlanRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<MealPlan, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id, unitId, 2),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(foodItem.Quantity, Is.EqualTo(3));
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
