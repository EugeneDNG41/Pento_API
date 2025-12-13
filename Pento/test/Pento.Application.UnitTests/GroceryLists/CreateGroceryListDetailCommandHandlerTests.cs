using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.GroceryLists.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListItems;
using Pento.Domain.GroceryLists;

namespace Pento.Application.UnitTests.GroceryLists;

internal sealed class CreateGroceryListDetailCommandHandlerTests
{
    private readonly IGenericRepository<GroceryList> _groceryListRepo;
    private readonly IGenericRepository<GroceryListItem> _groceryItemRepo;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;

    private readonly CreateGroceryListDetailCommandHandler _handler;

    public CreateGroceryListDetailCommandHandlerTests()
    {
        _groceryListRepo = Substitute.For<IGenericRepository<GroceryList>>();
        _groceryItemRepo = Substitute.For<IGenericRepository<GroceryListItem>>();
        _userContext = Substitute.For<IUserContext>();
        _uow = Substitute.For<IUnitOfWork>();
        _clock = Substitute.For<IDateTimeProvider>();

        _handler = new CreateGroceryListDetailCommandHandler(
            _groceryListRepo,
            _groceryItemRepo,
            _userContext,
            _uow,
            _clock);
    }

    private static CreateGroceryListDetailCommand CreateCommand()
        => new(
            "Weekly Grocery",
            new List<GroceryListItemRequest>
            {
                new(
                    FoodRefId: Guid.NewGuid(),
                    Quantity: 2,
                    CustomName: "Milk",
                    UnitId: Guid.NewGuid(),
                    Notes: "Low fat",
                    Priority: "High"
                ),
                new(
                    FoodRefId: Guid.NewGuid(),
                    Quantity: 5,
                    CustomName: "Eggs",
                    UnitId: null,
                    Notes: null,
                    Priority: "INVALID" // fallback → Medium
                )
            }
        );

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotInHousehold()
    {
        _userContext.HouseholdId.Returns((Guid?)null);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(),
            CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(GroceryListErrors.ForbiddenAccess));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ValidCommand_CreatesGroceryList()
    {
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(userId);
        _clock.UtcNow.Returns(DateTime.UtcNow);

        Result<Guid> result = await _handler.Handle(
            CreateCommand(),
            CancellationToken.None);

        Assert.That(result.IsSuccess);

        _groceryListRepo.Received(1).Add(
            Arg.Is<GroceryList>(g =>
                g.HouseholdId == householdId &&
                g.Name == "Weekly Grocery" &&
                g.CreatedBy == userId
            ));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ValidCommand_CreatesAllGroceryItems()
    {
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(userId);
        _clock.UtcNow.Returns(DateTime.UtcNow);

        await _handler.Handle(
            CreateCommand(),
            CancellationToken.None);

        _groceryItemRepo.Received(2)
            .Add(Arg.Any<GroceryListItem>());
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_ShouldParsePriority_AndFallbackToMedium()
    {
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(userId);
        _clock.UtcNow.Returns(DateTime.UtcNow);

        await _handler.Handle(
            CreateCommand(),
            CancellationToken.None);

        _groceryItemRepo.Received(1).Add(
            Arg.Is<GroceryListItem>(i =>
                i.CustomName == "Milk" &&
                i.Priority == GroceryItemPriority.High));

        _groceryItemRepo.Received(1).Add(
            Arg.Is<GroceryListItem>(i =>
                i.CustomName == "Eggs" &&
                i.Priority == GroceryItemPriority.Medium));
    }

    // ---------- TEST 5 ----------
    [Test]
    public async Task Handle_ValidCommand_SavesChangesOnce()
    {
        var householdId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(userId);
        _clock.UtcNow.Returns(DateTime.UtcNow);

        await _handler.Handle(
            CreateCommand(),
            CancellationToken.None);

        await _uow.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
