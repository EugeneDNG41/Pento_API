using Microsoft.AspNetCore.SignalR;
using NetTopologySuite.Geometries;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Sessions.Cancel;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.UnitTests.Trades;

internal sealed class CancelTradeSessionCommandHandlerTests
{
    private readonly IUserContext _userContext;
    private readonly IGenericRepository<TradeSession> _sessionRepo;
    private readonly IGenericRepository<TradeRequest> _requestRepo;
    private readonly IHubContext<MessageHub, IMessageClient> _hubContext;
    private readonly IHubClients<IMessageClient> _hubClients;
    private readonly IMessageClient _messageClient;
    private readonly IUnitOfWork _uow;

    private readonly CancelTradeSessionCommandHandler _handler;

    public CancelTradeSessionCommandHandlerTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _sessionRepo = Substitute.For<IGenericRepository<TradeSession>>();
        _requestRepo = Substitute.For<IGenericRepository<TradeRequest>>();
        _hubContext = Substitute.For<IHubContext<MessageHub, IMessageClient>>();
        _hubClients = Substitute.For<IHubClients<IMessageClient>>();
        _messageClient = Substitute.For<IMessageClient>();
        _uow = Substitute.For<IUnitOfWork>();

        _hubContext.Clients.Returns(_hubClients);
        _hubClients.Group(Arg.Any<string>()).Returns(_messageClient);

        _handler = new CancelTradeSessionCommandHandler(
            _userContext,
            _sessionRepo,
            _requestRepo,
            _hubContext,
            _uow
        );
    }

    private static CancelTradeSessionCommand CreateCommand(Guid sessionId)
        => new(sessionId);

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotInHousehold()
    {
        _userContext.HouseholdId.Returns((Guid?)null);

        Result result = await _handler.Handle(
            CreateCommand(Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(HouseholdErrors.NotInAnyHouseHold));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenSessionNotFound()
    {
        _userContext.HouseholdId.Returns(Guid.NewGuid());

        _sessionRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((TradeSession?)null);

        Result result = await _handler.Handle(
            CreateCommand(Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.SessionNotFound));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenForbiddenAccess()
    {
        var householdId = Guid.NewGuid();

        var session = TradeSession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);

        _userContext.HouseholdId.Returns(householdId);

        _sessionRepo.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        Result result = await _handler.Handle(
            CreateCommand(session.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.SessionForbiddenAccess));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenSessionNotOngoing()
    {
        var householdId = Guid.NewGuid();

        var session = TradeSession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            Guid.NewGuid(),
            DateTime.UtcNow);

        session.Cancel(); // not ongoing

        _userContext.HouseholdId.Returns(householdId);

        _sessionRepo.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        Result result = await _handler.Handle(
            CreateCommand(session.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.InvalidSessionState));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenRequestNotFound()
    {
        var householdId = Guid.NewGuid();

        var session = TradeSession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            Guid.NewGuid(),
            DateTime.UtcNow);

        _userContext.HouseholdId.Returns(householdId);

        _sessionRepo.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        _requestRepo.GetByIdAsync(session.TradeRequestId, Arg.Any<CancellationToken>())
            .Returns((TradeRequest?)null);

        Result result = await _handler.Handle(
            CreateCommand(session.Id),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.RequestNotFound));
    }

    // ---------- TEST 6 ----------
    [Test]
    public async Task Handle_OfferHousehold_CancelsRequest()
    {
        var householdId = Guid.NewGuid();
        var location = new Point(10, 20);
        var request = TradeRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            location,
            DateTime.UtcNow);

        var session = TradeSession.Create(
            Guid.NewGuid(),
            request.Id,
            householdId,
            Guid.NewGuid(),
            DateTime.UtcNow);

        _userContext.HouseholdId.Returns(householdId);

        _sessionRepo.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(request);

        Result result = await _handler.Handle(
            CreateCommand(session.Id),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(session.Status, Is.EqualTo(TradeSessionStatus.Cancelled));
        Assert.That(request.Status, Is.EqualTo(TradeRequestStatus.Cancelled));

        await _sessionRepo.Received(1).UpdateAsync(session);
        await _requestRepo.Received(1).UpdateAsync(request);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _messageClient.Received(1).TradeSessionCancelled(session.Id);
    }

    // ---------- TEST 7 ----------
    [Test]
    public async Task Handle_RequestHousehold_RejectsRequest()
    {
        var householdId = Guid.NewGuid();
        var location = new Point(10, 20);
        var request = TradeRequest.Create(
            Guid.NewGuid(),
            householdId,
            Guid.NewGuid(),
            location,
            DateTime.UtcNow);

        var session = TradeSession.Create(
            Guid.NewGuid(),
            request.Id,
            Guid.NewGuid(),
            householdId,
            DateTime.UtcNow);

        _userContext.HouseholdId.Returns(householdId);

        _sessionRepo.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(request);

        Result result = await _handler.Handle(
            CreateCommand(session.Id),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(request.Status, Is.EqualTo(TradeRequestStatus.Rejected));
    }
}
