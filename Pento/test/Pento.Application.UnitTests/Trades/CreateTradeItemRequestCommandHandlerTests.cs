using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.Trades;
using Pento.Application.Trades.Requests.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.UnitTests.Trades;

internal sealed class CreateTradeItemRequestCommandHandlerTests
{
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _clock;
    private readonly IGenericRepository<TradeRequest> _requestRepo;
    private readonly IGenericRepository<TradeItemRequest> _tradeItemRepo;
    private readonly IGenericRepository<TradeOffer> _offerRepo;
    private readonly IGenericRepository<FoodItem> _foodItemRepo;
    private readonly IConverterService _converter;
    private readonly IUnitOfWork _uow;

    private readonly CreateTradeItemRequestCommandHandler _handler;

    public CreateTradeItemRequestCommandHandlerTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _clock = Substitute.For<IDateTimeProvider>();
        _requestRepo = Substitute.For<IGenericRepository<TradeRequest>>();
        _tradeItemRepo = Substitute.For<IGenericRepository<TradeItemRequest>>();
        _offerRepo = Substitute.For<IGenericRepository<TradeOffer>>();
        _foodItemRepo = Substitute.For<IGenericRepository<FoodItem>>();
        _converter = Substitute.For<IConverterService>();
        _uow = Substitute.For<IUnitOfWork>();

        _handler = new CreateTradeItemRequestCommandHandler(
            _clock,
            _userContext,
            _requestRepo,
            _tradeItemRepo,
            _offerRepo,
            _foodItemRepo,
            _converter,
            _uow
        );
    }

    private static CreateTradeItemRequestCommand CreateCommand(Guid offerId, Guid foodItemId)
        => new(
            TradeOfferId: offerId,
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
    public async Task Handle_ShouldFail_WhenUserIdIsEmpty()
    {
        _userContext.UserId.Returns(Guid.Empty);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotInHousehold()
    {
        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns((Guid?)null);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(HouseholdErrors.NotInAnyHouseHold));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenOfferNotFound()
    {
        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns(Guid.NewGuid());

        _offerRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((TradeOffer?)null);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.OfferNotFound));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenTradingWithinSameHousehold()
    {
        var householdId = Guid.NewGuid();

        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns(householdId);

        var offer = TradeOffer.Create(
            userId: Guid.NewGuid(),
            householdId: householdId,
            startDate: DateTime.UtcNow,
            endDate: DateTime.UtcNow.AddDays(1),
            pickupOption: PickupOption.InPerson,
            createdOn: DateTime.UtcNow);

        _offerRepo.GetByIdAsync(offer.Id, Arg.Any<CancellationToken>())
            .Returns(offer);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(offer.Id, Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.CannotTradeWithinHousehold));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenOfferNotOpen()
    {
        var householdId = Guid.NewGuid();

        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns(householdId);

        var offer = TradeOffer.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            PickupOption.InPerson,
            DateTime.UtcNow);


        _offerRepo.GetByIdAsync(offer.Id, Arg.Any<CancellationToken>())
            .Returns(offer);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(offer.Id, Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.InvalidOfferState));
    }

    // ---------- TEST 6 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenDuplicatePendingRequestExists()
    {
        var householdId = Guid.NewGuid();
        var offerId = Guid.NewGuid();

        _userContext.UserId.Returns(Guid.NewGuid());
        _userContext.HouseholdId.Returns(householdId);

        var offer = TradeOffer.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            PickupOption.InPerson,
            DateTime.UtcNow);

        _offerRepo.GetByIdAsync(offerId, Arg.Any<CancellationToken>())
            .Returns(offer);

 


        _requestRepo.FindAsync(
                 Arg.Any<Expression<Func<TradeRequest, bool>>>(),
                 Arg.Any<CancellationToken>())
                 .Returns(Enumerable.Empty<TradeRequest>());

        Result<Guid> result = await _handler.Handle(
            CreateCommand(offerId, Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.DuplicateRequest));
    }

    // ---------- TEST 7 ----------
    [Test]
    public async Task Handle_ValidCommand_CreatesRequestAndReservesFood()
    {
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        _userContext.UserId.Returns(userId);
        _userContext.HouseholdId.Returns(householdId);
        _clock.UtcNow.Returns(DateTime.UtcNow);

        var offer = TradeOffer.Create(
            Guid.NewGuid(),
            Guid.NewGuid(), 
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            PickupOption.InPerson,
            DateTime.UtcNow);

        _offerRepo.GetByIdAsync(offer.Id, Arg.Any<CancellationToken>())
            .Returns(offer);

        _requestRepo.FindAsync(
              Arg.Any<Expression<Func<TradeRequest, bool>>>(),
              Arg.Any<CancellationToken>())
              .Returns(Enumerable.Empty<TradeRequest>());

        var foodItem = FoodItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            "Milk",
            null,
            5,
            unitId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
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
            CreateCommand(offer.Id, foodItem.Id),
            CancellationToken.None);

        Assert.That(result.IsSuccess);

        _requestRepo.Received(1).Add(Arg.Any<TradeRequest>());
        _tradeItemRepo.Received(1).Add(Arg.Any<TradeItemRequest>());
        await _foodItemRepo.Received(1).UpdateAsync(foodItem);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
