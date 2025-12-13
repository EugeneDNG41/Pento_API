using System.Linq.Expressions;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Trades.Requests.Accept;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.Application.UnitTests.Trades;

internal sealed class AcceptTradeRequestCommandHandlerTests
{
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _clock;

    private readonly IGenericRepository<TradeOffer> _offerRepo;
    private readonly IGenericRepository<TradeRequest> _requestRepo;
    private readonly IGenericRepository<TradeSession> _sessionRepo;
    private readonly IGenericRepository<TradeItemOffer> _offerItemRepo;
    private readonly IGenericRepository<TradeItemRequest> _requestItemRepo;
    private readonly IGenericRepository<TradeSessionItem> _sessionItemRepo;

    private readonly IHubContext<MessageHub, IMessageClient> _hub;
    private readonly IHubClients<IMessageClient> _clients;
    private readonly IMessageClient _client;

    private readonly IUnitOfWork _uow;

    private readonly AcceptTradeRequestCommandHandler _handler;

    public AcceptTradeRequestCommandHandlerTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _clock = Substitute.For<IDateTimeProvider>();

        _offerRepo = Substitute.For<IGenericRepository<TradeOffer>>();
        _requestRepo = Substitute.For<IGenericRepository<TradeRequest>>();
        _sessionRepo = Substitute.For<IGenericRepository<TradeSession>>();
        _offerItemRepo = Substitute.For<IGenericRepository<TradeItemOffer>>();
        _requestItemRepo = Substitute.For<IGenericRepository<TradeItemRequest>>();
        _sessionItemRepo = Substitute.For<IGenericRepository<TradeSessionItem>>();

        _hub = Substitute.For<IHubContext<MessageHub, IMessageClient>>();
        _clients = Substitute.For<IHubClients<IMessageClient>>();
        _client = Substitute.For<IMessageClient>();

        _hub.Clients.Returns(_clients);
        _clients.Groups(Arg.Any<string[]>()).Returns(_client);

        _uow = Substitute.For<IUnitOfWork>();

        _handler = new AcceptTradeRequestCommandHandler(
            _clock,
            _userContext,
            _offerRepo,
            _requestRepo,
            _sessionRepo,
            _offerItemRepo,
            _requestItemRepo,
            _sessionItemRepo,
            _hub,
            _uow
        );
    }

    private static AcceptTradeRequestCommand CreateCommand(Guid offerId, Guid requestId)
        => new(offerId, requestId);

    // ---------- FAILURE ----------
    [Test]
    public async Task Handle_ShouldFail_WhenOfferNotFound()
    {
        _offerRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((TradeOffer?)null);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(TradeErrors.OfferNotFound));
    }

    // ---------- SUCCESS ----------
    [Test]
    public async Task Handle_ValidCommand_CreatesTradeSessionAndSessionItems()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();

        var offerHouseholdId = Guid.NewGuid();
        var requestHouseholdId = Guid.NewGuid();

        _userContext.UserId.Returns(ownerId);
        _clock.UtcNow.Returns(DateTime.UtcNow);

        var offer = TradeOffer.Create(
            userId: ownerId,
            householdId: offerHouseholdId,
            startDate: DateTime.UtcNow,
            endDate: DateTime.UtcNow.AddDays(1),
            pickupOption: PickupOption.InPerson,
            createdOn: DateTime.UtcNow);

        var request = TradeRequest.Create(
            userId: requesterId,
            householdId: requestHouseholdId,
            tradeOfferId: offer.Id,
            createdOn: DateTime.UtcNow);

        _offerRepo.GetByIdAsync(offer.Id, Arg.Any<CancellationToken>())
            .Returns(offer);

        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(request);

        _sessionRepo.AnyAsync(
            Arg.Any<Expression<Func<TradeSession, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(false);

        _offerItemRepo.FindAsync(
            Arg.Any<Expression<Func<TradeItemOffer, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                TradeItemOffer.Create(Guid.NewGuid(), 1, Guid.NewGuid(), offer.Id)
            });

        _requestItemRepo.FindAsync(
            Arg.Any<Expression<Func<TradeItemRequest, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[]
            {
                TradeItemRequest.Create(Guid.NewGuid(), 2, Guid.NewGuid(), request.Id)
            });

        // Act
        Result<Guid> result = await _handler.Handle(
            CreateCommand(offer.Id, request.Id),
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess);

        _sessionRepo.Received(1).Add(Arg.Any<TradeSession>());
        _sessionItemRepo.Received(1)
            .AddRange(Arg.Is<IEnumerable<TradeSessionItem>>(x => x.Count() == 2));

        await _uow.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await _client.Received(1)
            .TradeSessionCreated(Arg.Any<Guid>());
    }
}
