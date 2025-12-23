using NetTopologySuite.Geometries;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.Trades;
using Pento.Application.Trades.Offers.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Units;

namespace Pento.Application.UnitTests.Trades;

internal sealed class CreateTradeItemOfferCommandHandlerTests
{
    private readonly IUserContext _userContext;
    private readonly IGenericRepository<TradeOffer> _offerRepo;
    private readonly IGenericRepository<TradeItemOffer> _tradeItemRepo;
    private readonly IGenericRepository<FoodItem> _foodItemRepo;
    private readonly IConverterService _converter;
    private readonly IDateTimeProvider _clock;
    private readonly IUnitOfWork _uow;

    private readonly CreateTradeItemOfferCommandHandler _handler;

    public CreateTradeItemOfferCommandHandlerTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _offerRepo = Substitute.For<IGenericRepository<TradeOffer>>();
        _tradeItemRepo = Substitute.For<IGenericRepository<TradeItemOffer>>();
        _foodItemRepo = Substitute.For<IGenericRepository<FoodItem>>();
        _converter = Substitute.For<IConverterService>();
        _clock = Substitute.For<IDateTimeProvider>();
        _uow = Substitute.For<IUnitOfWork>();

        _handler = new CreateTradeItemOfferCommandHandler(
            _userContext,
            _offerRepo,
            _tradeItemRepo,
            _foodItemRepo,
            _converter,
            _clock,
            _uow
        );
    }

    private static CreateTradeItemOfferCommand CreateCommand(Guid foodItemId)
        => new(
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(3),
            PickupOption: PickupOption.InPerson,
            new Point(0, 0),
            Items: new List<AddTradeItemDto>
            {
                new(
                    FoodItemId: foodItemId,
                    Quantity: 2,
                    UnitId: Guid.NewGuid()
                )
            }
        );

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
    public async Task Handle_ShouldFail_WhenFoodItemNotFound()
    {
        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns(Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((FoodItem?)null);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.NotFound));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenFoodItemNotInHousehold()
    {
        var householdId = Guid.NewGuid();

        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns(householdId);

        var foodItem = FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(), // ❌ khác household
            "Milk",
            null,
            5,
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            null,
            Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.ForbiddenAccess));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenConverterFails()
    {
        var householdId = Guid.NewGuid();

        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns(householdId);

        var foodItem = FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            "Milk",
            null,
            5,
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            null,
            Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _converter.ConvertAsync(
            Arg.Any<decimal>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>())
            .Returns(Result.Failure<decimal>(UnitErrors.InvalidConversion));

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(UnitErrors.InvalidConversion));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenQuantityExceedsAvailable()
    {
        var householdId = Guid.NewGuid();

        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns(householdId);

        var foodItem = FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            "Milk",
            null,
            1, // chỉ có 1
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            null,
            Guid.NewGuid());

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _converter.ConvertAsync(
            Arg.Any<decimal>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<decimal>(2))); 

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(FoodItemErrors.InsufficientQuantity));
    }

    // ---------- TEST 6 ----------
    [Test]
    public async Task Handle_ValidCommand_CreatesOfferAndReservesFood()
    {
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        _userContext.UserId.Returns(userId);
        _userContext.HouseholdId.Returns(householdId);
        _clock.UtcNow.Returns(DateTime.UtcNow);

        var foodItem = FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            "Milk",
            null,
            5,
            unitId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            null,
            userId);

        _foodItemRepo.GetByIdAsync(foodItem.Id, Arg.Any<CancellationToken>())
            .Returns(foodItem);

        _converter.ConvertAsync(
            Arg.Any<decimal>(),
            Arg.Any<Guid>(),
            unitId,
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<decimal>(2)));

        Result<Guid> result = await _handler.Handle(
            CreateCommand(foodItem.Id),
            CancellationToken.None);

        Assert.That(result.IsSuccess);

        _offerRepo.Received(1).Add(Arg.Any<TradeOffer>());
        _tradeItemRepo.Received(1).Add(Arg.Any<TradeItemOffer>());
        await _foodItemRepo.Received(1).UpdateAsync(foodItem);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
