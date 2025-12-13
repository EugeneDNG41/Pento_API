using NSubstitute;
using NUnit.Framework;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.MealPlans.Create.FromRecipe;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.MealPlanRecipe;
using Pento.Domain.MealPlans;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;
using Pento.Domain.Units;

namespace Pento.Application.UnitTests.MealPlans;

internal sealed class CreateMealPlanFromRecipeConfirmTests
{
    private IGenericRepository<Recipe> _recipeRepo;
    private IGenericRepository<FoodItem> _foodItemRepo;
    private IGenericRepository<RecipeIngredient> _ingredientRepo;
    private IGenericRepository<MealPlan> _mealPlanRepo;
    private IGenericRepository<FoodReference> _foodRefRepo;
    private IGenericRepository<Unit> _unitRepo;
    private IGenericRepository<MealPlanRecipe> _mealPlanRecipeRepo;
    private IGenericRepository<FoodItemMealPlanReservation> _reservationRepo;
    private IGenericRepository<Compartment> _compartmentRepo;
    private IConverterService _converter;
    private IUserContext _userContext;
    private IDateTimeProvider _clock;
    private IUnitOfWork _uow;

    private CreateMealPlanFromRecipeConfirmCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _recipeRepo = Substitute.For<IGenericRepository<Recipe>>();
        _foodItemRepo = Substitute.For<IGenericRepository<FoodItem>>();
        _ingredientRepo = Substitute.For<IGenericRepository<RecipeIngredient>>();
        _mealPlanRepo = Substitute.For<IGenericRepository<MealPlan>>();
        _foodRefRepo = Substitute.For<IGenericRepository<FoodReference>>();
        _unitRepo = Substitute.For<IGenericRepository<Unit>>();
        _mealPlanRecipeRepo = Substitute.For<IGenericRepository<MealPlanRecipe>>();
        _reservationRepo = Substitute.For<IGenericRepository<FoodItemMealPlanReservation>>();
        _compartmentRepo = Substitute.For<IGenericRepository<Compartment>>();
        _converter = Substitute.For<IConverterService>();
        _userContext = Substitute.For<IUserContext>();
        _clock = Substitute.For<IDateTimeProvider>();
        _uow = Substitute.For<IUnitOfWork>();

        _clock.UtcNow.Returns(DateTime.UtcNow);

        _handler = new CreateMealPlanFromRecipeConfirmCommandHandler(
            _recipeRepo,
            _foodItemRepo,
            _ingredientRepo,
            _mealPlanRepo,
            _foodRefRepo,
            _unitRepo,
            _mealPlanRecipeRepo,
            _reservationRepo,
            _compartmentRepo,
            _converter,
            _userContext,
            _clock,
            _uow);
    }

    private static CreateMealPlanFromRecipeConfirmCommand CreateCommand(Guid recipeId)
        => new(
            RecipeId: recipeId,
            MealType: MealType.Dinner,
            ScheduledDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Servings: 2
        );

    // ---------- TEST 1 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenUserNotInHousehold()
    {
        _userContext.HouseholdId.Returns((Guid?)null);

        Result<MealPlanAutoReserveResult> result = await _handler.Handle(CreateCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(HouseholdErrors.NotInAnyHouseHold));
    }

    // ---------- TEST 2 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenRecipeNotFound()
    {
        _userContext.HouseholdId.Returns(Guid.NewGuid());
        _recipeRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Recipe?)null);

        Result<MealPlanAutoReserveResult> result = await _handler.Handle(CreateCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(RecipeErrors.NotFound));
    }

    // ---------- TEST 3 ----------
    [Test]
    public async Task Handle_ShouldFail_WhenRecipeAlreadyAssigned()
    {
        var householdId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var mealPlanId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(Guid.NewGuid());

        _recipeRepo.GetByIdAsync(recipeId, Arg.Any<CancellationToken>())
            .Returns(Recipe.Create("Soup", "desc", TimeRequirement.Create(1, 1), null, 2, null, null, Guid.NewGuid(), true, DateTime.UtcNow));

        _ingredientRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<RecipeIngredient, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        _mealPlanRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<MealPlan, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([
                MealPlan.Create(householdId, "Dinner", MealType.Dinner, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 2, null, Guid.NewGuid(), DateTime.UtcNow)
            ]);

        _mealPlanRecipeRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<MealPlanRecipe, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([MealPlanRecipe.Create(mealPlanId, recipeId)]);

        Result<MealPlanAutoReserveResult> result = await _handler.Handle(CreateCommand(recipeId), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo(MealPlanErrors.RecipeAlreadyAssigned));
    }

    // ---------- TEST 4 ----------
    [Test]
    public async Task Handle_HappyPath_ShouldCreateMealPlanAndReserve()
    {
        var householdId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        _userContext.HouseholdId.Returns(householdId);
        _userContext.UserId.Returns(Guid.NewGuid());

        var recipe = Recipe.Create("Pasta", "desc", TimeRequirement.Create(1, 1), null, 2, null, null, Guid.NewGuid(), true, DateTime.UtcNow);
        _recipeRepo.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);

        var ingredient = RecipeIngredient.Create(recipeId, Guid.NewGuid(), 1, unitId, null, DateTime.UtcNow);
        _ingredientRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<RecipeIngredient, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([ingredient]);

        _foodRefRepo.GetByIdAsync(ingredient.FoodRefId, Arg.Any<CancellationToken>())
            .Returns(new FoodReference(
                ingredient.FoodRefId,
                "Tomato",
                FoodGroup.FruitsVegetables,
                null,
                null,
                null,
                "usda",
                0, 0, 0,
                null,
                null,
                UnitType.Count,
                DateTime.UtcNow));

        _unitRepo.GetByIdAsync(unitId, Arg.Any<CancellationToken>())
            .Returns(new Unit(unitId, "Piece", "pc", 1, UnitType.Count));

        var foodItem = FoodItem.Create(
            ingredient.FoodRefId,
            Guid.NewGuid(),
            householdId,
            "Tomato",
            null,
            5,
            unitId,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            null,
            Guid.NewGuid());

        _foodItemRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<FoodItem, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([foodItem]);

        _mealPlanRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<MealPlan, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        _mealPlanRecipeRepo.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<MealPlanRecipe, bool>>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        Result<MealPlanAutoReserveResult> result = await _handler.Handle(CreateCommand(recipeId), CancellationToken.None);

        Assert.That(result.IsSuccess);

        _mealPlanRepo.Received(1).Add(Arg.Any<MealPlan>());
        _mealPlanRecipeRepo.Received(1).Add(Arg.Any<MealPlanRecipe>());
        _reservationRepo.Received().Add(Arg.Any<FoodItemMealPlanReservation>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
