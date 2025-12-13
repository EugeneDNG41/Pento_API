using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Households.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Storages;
using Pento.Domain.Users;

namespace Pento.Application.UnitTests.Households;

internal sealed class CreateHouseholdTests
{
    private readonly IDateTimeProvider _dateTime;
    private readonly IUserContext _userContext;
    private readonly IGenericRepository<Household> _householdRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<Role> _roleRepo;
    private readonly IGenericRepository<Storage> _storageRepo;
    private readonly IGenericRepository<Compartment> _compartmentRepo;
    private readonly IUnitOfWork _uow;

    private readonly CreateHouseholdCommandHandler _handler;

    public CreateHouseholdTests()
    {
        _dateTime = Substitute.For<IDateTimeProvider>();
        _userContext = Substitute.For<IUserContext>();
        _householdRepo = Substitute.For<IGenericRepository<Household>>();
        _userRepo = Substitute.For<IGenericRepository<User>>();
        _roleRepo = Substitute.For<IGenericRepository<Role>>();
        _storageRepo = Substitute.For<IGenericRepository<Storage>>();
        _compartmentRepo = Substitute.For<IGenericRepository<Compartment>>();
        _uow = Substitute.For<IUnitOfWork>();

        _handler = new CreateHouseholdCommandHandler(
            _dateTime,
            _userContext,
            _householdRepo,
            _userRepo,
            _roleRepo,
            _storageRepo,
            _compartmentRepo,
            _uow);
    }

    private static CreateHouseholdCommand CreateCommand()
        => new("My Household");

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotFound()
    {
        // Arrange
        _userContext.UserId.Returns(Guid.NewGuid());

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        // Act
        Result<string> result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_UserHasPreviousHousehold_AssignsNewHeadIfMissing()
    {
        // Arrange
        var oldHouseholdId = Guid.NewGuid();

        var currentUser = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);
        currentUser.SetHouseholdId(oldHouseholdId);

        var otherUser = User.Create("c@d.com", "C", "D", "id2", DateTime.UtcNow);
        otherUser.SetHouseholdId(oldHouseholdId);

        _userContext.UserId.Returns(currentUser.Id);
        _dateTime.UtcNow.Returns(DateTime.UtcNow);

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { currentUser });

        _roleRepo.FindAsync(
            Arg.Any<Expression<Func<Role, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { Role.HouseholdHead });

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { otherUser });

        // Act
        await _handler.Handle(CreateCommand(), CancellationToken.None);

        // Assert
        Assert.That(otherUser.Roles.Any(r => r.Name == Role.HouseholdHead.Name));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ValidCommand_CreatesHouseholdAndAssignsHead()
    {
        // Arrange
        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);

        _userContext.UserId.Returns(user.Id);
        _dateTime.UtcNow.Returns(DateTime.UtcNow);

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { user });

        _roleRepo.FindAsync(
            Arg.Any<Expression<Func<Role, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { Role.HouseholdHead });

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        // Act
        Result<string> result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess);
        Assert.That(user.HouseholdId, Is.Not.Null);
        Assert.That(user.Roles.Any(r => r.Name == Role.HouseholdHead.Name));
        _householdRepo.Received(1).Add(Arg.Any<Household>());
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ValidCommand_CreatesDefaultStoragesAndCompartments()
    {
        // Arrange
        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);

        _userContext.UserId.Returns(user.Id);
        _dateTime.UtcNow.Returns(DateTime.UtcNow);

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { user });

        _roleRepo.FindAsync(
            Arg.Any<Expression<Func<Role, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { Role.HouseholdHead });

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        // Act
        await _handler.Handle(CreateCommand(), CancellationToken.None);

        // Assert
        _storageRepo.Received(1).AddRange(Arg.Is<IEnumerable<Storage>>(s => s.Count() == 3));
        _compartmentRepo.Received(1).AddRange(Arg.Is<IEnumerable<Compartment>>(c => c.Count() == 3));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ValidCommand_SavesChangesOnce()
    {
        // Arrange
        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);

        _userContext.UserId.Returns(user.Id);
        _dateTime.UtcNow.Returns(DateTime.UtcNow);

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { user });

        _roleRepo.FindAsync(
            Arg.Any<Expression<Func<Role, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(new[] { Role.HouseholdHead });

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        // Act
        await _handler.Handle(CreateCommand(), CancellationToken.None);

        // Assert
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
