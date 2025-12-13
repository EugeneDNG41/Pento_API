using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Households.Leave;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.UnitTests.Households;

internal sealed class LeaveHouseholdTests
{
    private readonly IUserContext _userContext;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IUnitOfWork _uow;

    private readonly LeaveHouseholdCommandHandler _handler;

    public LeaveHouseholdTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _userRepo = Substitute.For<IGenericRepository<User>>();
        _uow = Substitute.For<IUnitOfWork>();

        _handler = new LeaveHouseholdCommandHandler(
            _userContext,
            _userRepo,
            _uow);
    }

    private static LeaveHouseholdCommand CreateCommand()
        => new();

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldReturnSuccess_WhenUserNotInHousehold()
    {
        // Arrange
        _userContext.HouseholdId.Returns((Guid?)null);

        // Act
        Result result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess);
        await _uow.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _userRepo.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userContext.UserId.Returns(userId);
        _userContext.HouseholdId.Returns(Guid.NewGuid());

        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        Result result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound));
        await _uow.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ValidCommand_RemovesHouseholdFromUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();

        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);
        user.SetHouseholdId(householdId);

        _userContext.UserId.Returns(userId);
        _userContext.HouseholdId.Returns(householdId);

        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess);
        Assert.That(user.HouseholdId, Is.Null);

        _userRepo.Received(1).Update(user);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
