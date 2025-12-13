using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.MealPlans.Reserve.Fullfill;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.Households;

namespace Pento.Application.UnitTests.MealPlans;

internal sealed class FulfillMealPlanCommandHandlerTests
{
    private readonly IGenericRepository<FoodItemMealPlanReservation> _reservationRepo;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _uow;

    private readonly FulfillMealPlanCommandHandler _handler;

    public FulfillMealPlanCommandHandlerTests()
    {
        _reservationRepo = Substitute.For<IGenericRepository<FoodItemMealPlanReservation>>();
        _userContext = Substitute.For<IUserContext>();
        _uow = Substitute.For<IUnitOfWork>();

        _handler = new FulfillMealPlanCommandHandler(
            _reservationRepo,
            _userContext,
            _uow);
    }

    private static FulfillMealPlanCommand CreateCommand(Guid mealPlanId)
        => new(mealPlanId);

    private static FoodItemMealPlanReservation CreateReservation(
        Guid mealPlanId,
        Guid householdId,
        ReservationStatus status)
    {
        return new FoodItemMealPlanReservation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            DateTime.UtcNow,
            quantity: 2,
            unitId: Guid.NewGuid(),
            reservationStatus: status,
            reservationFor: ReservationFor.MealPlan,
            mealplanId: mealPlanId
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
            CreateReservation(mealPlanId, householdId, ReservationStatus.Pending),
            CreateReservation(mealPlanId, householdId, ReservationStatus.Fulfilled)
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
    public async Task Handle_HappyPath_FulfillsAllReservations()
    {
        var householdId = Guid.NewGuid();
        var mealPlanId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(userId);

        FoodItemMealPlanReservation[] reservations = new[]
        {
            CreateReservation(mealPlanId, householdId, ReservationStatus.Pending),
            CreateReservation(mealPlanId, householdId, ReservationStatus.Pending)
        };

        _reservationRepo.FindAsync(
            Arg.Any<System.Linq.Expressions.Expression<Func<FoodItemMealPlanReservation, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(reservations);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(mealPlanId),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value, Is.EqualTo(mealPlanId));

        Assert.That(reservations.All(r => r.Status == ReservationStatus.Fulfilled));
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
