using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.MealPlans.Reserve.Cancel;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Units;

namespace Pento.Application.UnitTests.MealPlans;

internal sealed class CancelMealPlanCommandHandlerTests
{
    private readonly IConverterService _converter;
    private readonly IGenericRepository<FoodItemMealPlanReservation> _reservationRepo;
    private readonly IGenericRepository<FoodItem> _foodItemRepo;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _uow;

    private readonly CancelMealPlanCommandHandler _handler;

    public CancelMealPlanCommandHandlerTests()
    {
        _converter = Substitute.For<IConverterService>();
        _reservationRepo = Substitute.For<IGenericRepository<FoodItemMealPlanReservation>>();
        _foodItemRepo = Substitute.For<IGenericRepository<FoodItem>>();
        _userContext = Substitute.For<IUserContext>();
        _uow = Substitute.For<IUnitOfWork>();

        _handler = new CancelMealPlanCommandHandler(
            _converter,
            _reservationRepo,
            _foodItemRepo,
            _userContext,
            _uow);
    }

    private static CancelMealPlanCommand CreateCommand(Guid mealPlanId)
        => new(mealPlanId);

    private static FoodItemMealPlanReservation CreateReservation(
        Guid mealPlanId,
        Guid householdId,
        Guid foodItemId,
        ReservationStatus status,
        decimal quantity,
        Guid unitId)
    {
        return new FoodItemMealPlanReservation(
            Guid.NewGuid(),
            foodItemId,
            householdId,
            DateTime.UtcNow,
            quantity,
            unitId,
            status,
            ReservationFor.MealPlan,
            mealPlanId
        );
    }

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotInHousehold()
    {
        _userContext.HouseholdId.Returns((Guid?)null);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(HouseholdErrors.NotInAnyHouseHold));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenNoReservationsFound()
    {
        var householdId = Guid.NewGuid();
        _userContext.HouseholdId.Returns(householdId);

        _reservationRepo.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<FoodItemMealPlanReservation, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns([]);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemReservationErrors.NotFound));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenAnyReservationNotPending()
    {
        var householdId = Guid.NewGuid();
        var mealPlanId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);

        FoodItemMealPlanReservation[] reservations = new[]
        {
            CreateReservation(mealPlanId, householdId, Guid.NewGuid(), ReservationStatus.Pending, 1, Guid.NewGuid()),
            CreateReservation(mealPlanId, householdId, Guid.NewGuid(), ReservationStatus.Fulfilled, 1, Guid.NewGuid())
        };

        _reservationRepo.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<FoodItemMealPlanReservation, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(reservations);


        Result<Guid> result = await _handler.Handle(
            CreateCommand(mealPlanId),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemReservationErrors.InvalidState));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenFoodItemsNotFound()
    {
        var householdId = Guid.NewGuid();
        var mealPlanId = Guid.NewGuid();
        var foodItemId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);

        FoodItemMealPlanReservation[] reservations = new[]
        {
            CreateReservation(mealPlanId, householdId, foodItemId, ReservationStatus.Pending, 1, Guid.NewGuid())
        };

        _reservationRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<FoodItemMealPlanReservation, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(reservations);

        _foodItemRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<FoodItem, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([]); // not found

        Result<Guid> result = await _handler.Handle(
            CreateCommand(mealPlanId),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.NotFound));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenConversionFails()
    {
        var householdId = Guid.NewGuid();
        var mealPlanId = Guid.NewGuid();
        var foodItemId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);

        FoodItemMealPlanReservation reservation = CreateReservation(
            mealPlanId, householdId, foodItemId,
            ReservationStatus.Pending, 2, unitId);

        var foodItem = new FoodItem(
            foodItemId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            "Milk",
            null,
            5,
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            null,
            Guid.NewGuid());

        _reservationRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<FoodItemMealPlanReservation, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([reservation]);

        _foodItemRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<FoodItem, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([foodItem]);

        _converter.ConvertAsync(
            reservation.Quantity,
            reservation.UnitId,
            foodItem.UnitId,
            Arg.Any<CancellationToken>())
            .Returns(Result.Failure<decimal>(UnitErrors.InvalidConversion));

        Result<Guid> result = await _handler.Handle(
            CreateCommand(mealPlanId),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(UnitErrors.InvalidConversion));
    }

    // ---------- TEST 6 ----------
    [Test]
    public async Task Handle_HappyPath_CancelsReservationsAndRestoresQuantity()
    {
        var householdId = Guid.NewGuid();
        var mealPlanId = Guid.NewGuid();
        var foodItemId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);

        FoodItemMealPlanReservation reservation = CreateReservation(
            mealPlanId, householdId, foodItemId,
            ReservationStatus.Pending, 2, Guid.NewGuid());

        var foodItem = new FoodItem(
            foodItemId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            "Milk",
            null,
            quantity: 3,
            unitId: Guid.NewGuid(),
            expirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            notes: null,
            addedBy: Guid.NewGuid());

        _reservationRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<FoodItemMealPlanReservation, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([reservation]);

        _foodItemRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<FoodItem, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([foodItem]);

        _converter.ConvertAsync(
            reservation.Quantity,
            reservation.UnitId,
            foodItem.UnitId,
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<decimal>(2m)));

        Result<Guid> result = await _handler.Handle(
            CreateCommand(mealPlanId),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value, Is.EqualTo(mealPlanId));
        Assert.That(reservation.Status, Is.EqualTo(ReservationStatus.Cancelled));
        Assert.That(foodItem.Quantity, Is.EqualTo(5)); // 3 + 2

        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
