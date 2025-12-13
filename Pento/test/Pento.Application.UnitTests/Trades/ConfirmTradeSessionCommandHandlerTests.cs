using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Sessions.Confirm;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.UnitTests.Trades;

internal sealed class ConfirmTradeSessionCommandHandlerTests
{
    private readonly IUserContext _userContext;
    private readonly IHubContext<MessageHub, IMessageClient> _hubContext;
    private readonly IHubClients<IMessageClient> _hubClients;
    private readonly IMessageClient _messageClient;
    private readonly IGenericRepository<TradeSession> _sessionRepo;
    private readonly IGenericRepository<TradeSessionItem> _sessionItemRepo;
    private readonly IUnitOfWork _uow;

    private readonly ConfirmTradeSessionCommandHandler _handler;

    public ConfirmTradeSessionCommandHandlerTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _hubContext = Substitute.For<IHubContext<MessageHub, IMessageClient>>();
        _hubClients = Substitute.For<IHubClients<IMessageClient>>();
        _messageClient = Substitute.For<IMessageClient>();
        _sessionRepo = Substitute.For<IGenericRepository<TradeSession>>();
        _sessionItemRepo = Substitute.For<IGenericRepository<TradeSessionItem>>();
        _uow = Substitute.For<IUnitOfWork>();

        _hubContext.Clients.Returns(_hubClients);
        _hubClients.Group(Arg.Any<string>()).Returns(_messageClient);

        _handler = new ConfirmTradeSessionCommandHandler(
            _userContext,
            _hubContext,
            _sessionRepo,
            _sessionItemRepo,
            _uow
        );
    }

    private static ConfirmTradeSessionCommand CreateCommand(Guid sessionId)
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
    public async Task Handle_OfferHousehold_ConfirmOnce_SetsConfirmedByOffer()
    {
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();

        var session = TradeSession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            Guid.NewGuid(),
            DateTime.UtcNow);

        _userContext.UserId.Returns(userId);
        _userContext.HouseholdId.Returns(householdId);

        _sessionRepo.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        Result result = await _handler.Handle(
            CreateCommand(session.Id),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(session.ConfirmedByOfferUserId, Is.EqualTo(userId));

        await _messageClient.Received(1)
            .TradeSessionConfirm(true, false);
    }

    // ---------- TEST 6 ----------
    [Test]
    public async Task Handle_OfferHousehold_ConfirmTwice_TogglesOff()
    {
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();

        var session = TradeSession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            Guid.NewGuid(),
            DateTime.UtcNow);

        session.ConfirmByOfferUser(userId);

        _userContext.UserId.Returns(userId);
        _userContext.HouseholdId.Returns(householdId);

        _sessionRepo.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        Result result = await _handler.Handle(
            CreateCommand(session.Id),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(session.ConfirmedByOfferUserId, Is.Null);
    }

    // ---------- TEST 7 ----------
    [Test]
    public async Task Handle_RequestHousehold_Confirm()
    {
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();

        var session = TradeSession.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            householdId,
            DateTime.UtcNow);

        _userContext.UserId.Returns(userId);
        _userContext.HouseholdId.Returns(householdId);

        _sessionRepo.GetByIdAsync(session.Id, Arg.Any<CancellationToken>())
            .Returns(session);

        Result result = await _handler.Handle(
            CreateCommand(session.Id),
            CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(session.ConfirmedByRequestUserId, Is.EqualTo(userId));
    }
}
