using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Households.Join;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.UnitTests.Households;

internal sealed class JoinHouseholdTests
{
    private readonly IUserContext _userContext;
    private readonly IGenericRepository<Household> _householdRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<Role> _roleRepo;
    private readonly IUnitOfWork _uow;

    private readonly JoinHouseholdCommandHandler _handler;

    public JoinHouseholdTests()
    {
        _userContext = Substitute.For<IUserContext>();
        _householdRepo = Substitute.For<IGenericRepository<Household>>();
        _userRepo = Substitute.For<IGenericRepository<User>>();
        _roleRepo = Substitute.For<IGenericRepository<Role>>();
        _uow = Substitute.For<IUnitOfWork>();

        _handler = new JoinHouseholdCommandHandler(
            _userContext,
            _householdRepo,
            _userRepo,
            _roleRepo,
            _uow);
    }

    private static JoinHouseholdCommand CreateCommand(string inviteCode = "ABC123")
        => new(inviteCode);

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotFound()
    {
        _userContext.UserId.Returns(Guid.NewGuid());

        _userRepo.FindIncludeAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            Arg.Any<Expression<Func<User, object>>>(),
            Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());

        Result result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenInviteCodeNotFound()
    {
        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);

        _userContext.UserId.Returns(user.Id);

        _userRepo.FindIncludeAsync(Arg.Any<Expression<Func<User, bool>>>(),
                                  Arg.Any<Expression<Func<User, object>>>(),
                                  Arg.Any<CancellationToken>())
            .Returns([user]);

        _householdRepo.FindAsync(
            Arg.Any<Expression<Func<Household, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Household>());

        Result result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(HouseholdErrors.InviteCodeNotFound));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenInviteCodeExpired()
    {
        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);
        var household = Household.Create("Home", DateTime.UtcNow.AddDays(-10), Guid.NewGuid());
        household.SetInviteCodeExpiration(DateTime.UtcNow.AddMinutes(-1));

        _userContext.UserId.Returns(user.Id);

        _userRepo.FindIncludeAsync(Arg.Any<Expression<Func<User, bool>>>(),
                                  Arg.Any<Expression<Func<User, object>>>(),
                                  Arg.Any<CancellationToken>())
            .Returns([user]);

        _householdRepo.FindAsync(Arg.Any<Expression<Func<Household, bool>>>(),
                                 Arg.Any<CancellationToken>())
            .Returns([household]);

        Result result = await _handler.Handle(CreateCommand(household.InviteCode!), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(HouseholdErrors.InviteCodeExpired));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenAlreadyInHousehold()
    {
        var household = Household.Create("Home", DateTime.UtcNow, Guid.NewGuid());
        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);
        user.SetHouseholdId(household.Id);

        _userContext.UserId.Returns(user.Id);

        _userRepo.FindIncludeAsync(Arg.Any<Expression<Func<User, bool>>>(),
                                  Arg.Any<Expression<Func<User, object>>>(),
                                  Arg.Any<CancellationToken>())
            .Returns([user]);

        _householdRepo.FindAsync(Arg.Any<Expression<Func<Household, bool>>>(),
                                 Arg.Any<CancellationToken>())
            .Returns([household]);

        Result result = await _handler.Handle(CreateCommand(household.InviteCode!), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(HouseholdErrors.AlreadyInThisHousehold));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenRolesNotFound()
    {
        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);
        var household = Household.Create("Home", DateTime.UtcNow, Guid.NewGuid());

        _userContext.UserId.Returns(user.Id);

        _userRepo.FindIncludeAsync(Arg.Any<Expression<Func<User, bool>>>(),
                                  Arg.Any<Expression<Func<User, object>>>(),
                                  Arg.Any<CancellationToken>())
            .Returns([user]);

        _householdRepo.FindAsync(Arg.Any<Expression<Func<Household, bool>>>(),
                                 Arg.Any<CancellationToken>())
            .Returns([household]);

        _roleRepo.FindAsync(Arg.Any<Expression<Func<Role, bool>>>(),
                            Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Role>());

        Result result = await _handler.Handle(CreateCommand(household.InviteCode!), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(RoleErrors.NotFoundToAssign));
    }

    // ---------- TEST 6 ----------
    [Test]
    public async Task Handle_ValidCommand_JoinsHouseholdAndAssignsRole()
    {
        var household = Household.Create("Home", DateTime.UtcNow, Guid.NewGuid());
        var user = User.Create("a@b.com", "A", "B", "id", DateTime.UtcNow);

        _userContext.UserId.Returns(user.Id);

        _userRepo.FindIncludeAsync(Arg.Any<Expression<Func<User, bool>>>(),
                                  Arg.Any<Expression<Func<User, object>>>(),
                                  Arg.Any<CancellationToken>())
            .Returns([user]);

        _householdRepo.FindAsync(Arg.Any<Expression<Func<Household, bool>>>(),
                                 Arg.Any<CancellationToken>())
            .Returns([household]);

        _roleRepo.FindAsync(r => r.Name == Role.HouseholdHead.Name, Arg.Any<CancellationToken>())
            .Returns([Role.HouseholdHead]);

        _roleRepo.FindAsync(r => r.Name == Role.HouseholdMember.Name, Arg.Any<CancellationToken>())
            .Returns([Role.HouseholdMember]);

        Result result = await _handler.Handle(CreateCommand(household.InviteCode!), CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(user.HouseholdId, Is.EqualTo(household.Id));
        Assert.That(user.Roles.Any(r => r.Name == Role.HouseholdMember.Name));
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
