using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Requests.Reject;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.Application.UnitTests.Trades;

internal sealed class RejectTradeRequestCommandHandlerTests
{
    private readonly IUserContext _userContext;
    private readonly IGenericRepository<TradeRequest> _requestRepo;
    private readonly IGenericRepository<TradeOffer> _offerRepo;
    private readonly IGenericRepository<TradeSession> _sessionRepo;
    private readonly IUnitOfWork _uow;

    private readonly RejectTradeRequestCommandHandler _handler;

    public RejectTradeRequestCommandHandlerTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _requestRepo = Substitute.For<IGenericRepository<TradeRequest>>();
        _offerRepo = Substitute.For<IGenericRepository<TradeOffer>>();
        _sessionRepo = Substitute.For<IGenericRepository<TradeSession>>();
        _uow = Substitute.For<IUnitOfWork>();

        _handler = new RejectTradeRequestCommandHandler(
            _userContext,
            _requestRepo,
            _offerRepo,
            _sessionRepo,
            _uow
        );
    }

    private static RejectTradeRequestCommand CreateCommand(Guid requestId)
        => new(requestId);

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenRequestNotFound()
    {
        _requestRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((TradeRequest?)null);

        Result result = await _handler.Handle(
            CreateCommand(Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.RequestNotFound));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenOfferNotFound()
    {
        var request = TradeRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);

        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(request);

        _offerRepo.GetByIdAsync(request.TradeOfferId, Arg.Any<CancellationToken>())
            .Returns((TradeOffer?)null);

        Result result = await _handler.Handle(
            CreateCommand(request.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.OfferNotFound));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotOwnerOfOfferHousehold()
    {
        var householdId = Guid.NewGuid();
        var otherHouseholdId = Guid.NewGuid();

        var request = TradeRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);

        var offer = TradeOffer.Create(
            userId: Guid.NewGuid(),
            householdId: otherHouseholdId,
            startDate: DateTime.UtcNow,
            endDate: DateTime.UtcNow.AddDays(1),
            pickupOption: PickupOption.InPerson,
            createdOn: DateTime.UtcNow);

        _userContext.HouseholdId.Returns(householdId);

        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(request);

        _offerRepo.GetByIdAsync(request.TradeOfferId, Arg.Any<CancellationToken>())
            .Returns(offer);

        Result result = await _handler.Handle(
            CreateCommand(request.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.OfferForbiddenAccess));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenRequestNotPending()
    {
        var householdId = Guid.NewGuid();

        var request = TradeRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);

        request.Reject(); // make it non-pending

        var offer = TradeOffer.Create(
            userId: Guid.NewGuid(),
            householdId: householdId,
            startDate: DateTime.UtcNow,
            endDate: DateTime.UtcNow.AddDays(1),
            pickupOption: PickupOption.InPerson,
            createdOn: DateTime.UtcNow);

        _userContext.HouseholdId.Returns(householdId);

        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(request);

        _offerRepo.GetByIdAsync(request.TradeOfferId, Arg.Any<CancellationToken>())
            .Returns(offer);

        Result result = await _handler.Handle(
            CreateCommand(request.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.InvalidRequestState));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ValidCommand_CancelsSessionsAndRejectsRequest()
    {
        var householdId = Guid.NewGuid();

        var request = TradeRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);

        var offer = TradeOffer.Create(
            userId: Guid.NewGuid(),
            householdId: householdId,
            startDate: DateTime.UtcNow,
            endDate: DateTime.UtcNow.AddDays(1),
            pickupOption: PickupOption.InPerson,
            createdOn: DateTime.UtcNow);

        var session1 = TradeSession.Create(
            offer.Id,
            request.Id,
            offer.HouseholdId,
            request.HouseholdId,
            DateTime.UtcNow);

        _userContext.HouseholdId.Returns(householdId);

        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(request);

        _offerRepo.GetByIdAsync(request.TradeOfferId, Arg.Any<CancellationToken>())
            .Returns(offer);

        _sessionRepo.FindAsync(
            Arg.Any<Expression<Func<TradeSession, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { session1 });

        Result result = await _handler.Handle(
            CreateCommand(request.Id),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(request.Status, Is.EqualTo(TradeRequestStatus.Rejected));

        _sessionRepo.Received(1).Update(session1);
        _requestRepo.Received(1).Update(request);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
