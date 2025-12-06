using Microsoft.AspNetCore.Mvc;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.GroceryLists;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.MealPlans.Events;
using Pento.Domain.Recipes;
using Pento.Domain.Recipes.Events;
using Pento.Domain.Storages;
using Pento.Domain.Units;
using Pento.Domain.UserActivities;
using Pento.Domain.Users;

namespace Pento.Application.EventHandlers;

internal sealed class FoodItemAddedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Unit> unitRepository,
    IGenericRepository<FoodItemLog> logRepository,
    IUnitOfWork unitOfWork)
    : DomainEventHandler<FoodItemAddedDomainEvent>
{
    public override async Task Handle(
        FoodItemAddedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(domainEvent.FoodItemId, cancellationToken);
        if (foodItem is null)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), FoodItemErrors.NotFound);
        }
        Unit? unit = await unitRepository.GetByIdAsync(domainEvent.UnitId, cancellationToken);
        if (unit is null)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), FoodItemErrors.InvalidMeasurementUnit);
        }
        var log = FoodItemLog.Create(
            foodItem.Id,
            foodItem.HouseholdId,
            domainEvent.UserId,
            domainEvent.Timestamp,
            FoodItemLogAction.Intake,
            domainEvent.Quantity,
            unit.Id);
        logRepository.Add(log);
        Result<UserActivity> intakeResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            foodItem.HouseholdId,
            ActivityCode.FOOD_ITEM_INTAKE.ToString(),
            foodItem.Id,
            cancellationToken);
        if (intakeResult.IsFailure)
        {
            throw new PentoException(nameof(FoodItemAddedEventHandler), intakeResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(intakeResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class HouseholdCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<Household> householdRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<HouseholdCreatedDomainEvent>
{
    public async override Task Handle(HouseholdCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Household? household = await householdRepository.GetByIdAsync(domainEvent.HouseholdId, cancellationToken);
        if (household == null)
        {
            throw new PentoException(nameof(HouseholdCreatedEventHandler), HouseholdErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            domainEvent.HouseholdId,
            ActivityCode.HOUSEHOLD_CREATE.ToString(),
            domainEvent.HouseholdId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(HouseholdCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class HouseholdJoinedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<Household> householdRepository,
    IGenericRepository<User> userRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<UserHouseholdJoinedDomainEvent>
{
    public async override Task Handle(UserHouseholdJoinedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Household? household = await householdRepository.GetByIdAsync(domainEvent.HouseholdId, cancellationToken);
        if (household == null)
        {
            throw new PentoException(nameof(HouseholdJoinedEventHandler), HouseholdErrors.NotFound);
        }
        User? user = await userRepository.GetByIdAsync(domainEvent.UserId, cancellationToken);
        if (user == null)
        {
            throw new PentoException(nameof(HouseholdJoinedEventHandler), UserErrors.NotFound);
        }
        Result<UserActivity> joinResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            domainEvent.HouseholdId,
            ActivityCode.HOUSEHOLD_JOIN.ToString(),
            domainEvent.HouseholdId,
            cancellationToken);
        if (joinResult.IsFailure)
        {
            throw new PentoException(nameof(HouseholdJoinedEventHandler), joinResult.Error);
        }
        Result milestoneCheckJoinResult = await milestoneService.CheckMilestoneAfterActivityAsync(joinResult.Value, cancellationToken);
        if (milestoneCheckJoinResult.IsFailure)
        {
            throw new PentoException(nameof(HouseholdJoinedEventHandler), milestoneCheckJoinResult.Error);
        }
        IEnumerable<User> otherMembers = await userRepository.FindAsync(u => u.HouseholdId == domainEvent.HouseholdId && u.Id != domainEvent.UserId,
            cancellationToken: cancellationToken);
        foreach (User member in otherMembers)
        {
            Result<UserActivity> joinedResult = await activityService.RecordActivityAsync(
                member.Id,
                domainEvent.HouseholdId,
                ActivityCode.HOUSEHOLD_MEMBER_JOINED.ToString(),
                domainEvent.UserId,
                cancellationToken);
            if (joinedResult.IsFailure)
            {
                throw new PentoException(nameof(HouseholdJoinedEventHandler), joinedResult.Error);
            }
            Result milestoneCheckJoinedResult = await milestoneService.CheckMilestoneAfterActivityAsync(joinedResult.Value, cancellationToken);
            if (milestoneCheckJoinedResult.IsFailure)
            {
                throw new PentoException(nameof(HouseholdJoinedEventHandler), milestoneCheckJoinedResult.Error);
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class HouseholdDeletedEventHandler(
    IGenericRepository<FoodItemReservation> foodItemReservationRepository,
    IGenericRepository<MealPlan> mealPlanRepository,
    IGenericRepository<FoodItem> foodItemRepository,     
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<GroceryList> groceryListRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<HouseholdDeletedDomainEvent>
{
    public async override Task Handle(HouseholdDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        IEnumerable<FoodItemReservation> foodItemReservations = await foodItemReservationRepository.FindAsync(fir => fir.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        foreach (FoodItemReservation reservation in foodItemReservations)
        {
            reservation.Delete();
            
        }
        foodItemReservationRepository.UpdateRange(foodItemReservations);

        IEnumerable<MealPlan> mealPlans = await mealPlanRepository.FindAsync(mp => mp.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        foreach (MealPlan mealPlan in mealPlans)
        {
            mealPlan.Delete();
        }
        mealPlanRepository.UpdateRange(mealPlans);

        IEnumerable<FoodItem> foodItems = await foodItemRepository.FindAsync(fi => fi.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        foreach (FoodItem foodItem in foodItems)
        {
            foodItem.Delete();
        }
        foodItemRepository.UpdateRange(foodItems);

        IEnumerable<Storage> storages = await storageRepository.FindAsync(s => s.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        foreach (Storage storage in storages)
        {
            storage.Delete(); 
        }
        foodItemRepository.UpdateRange(foodItems);

        IEnumerable<Compartment> compartments = await compartmentRepository.FindAsync(c => c.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        foreach (Compartment compartment in compartments)
        {
            compartment.Delete();
        }
        foodItemRepository.UpdateRange(foodItems);

        IEnumerable<GroceryList> groceryLists = await groceryListRepository.FindAsync(gl => gl.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        foreach (GroceryList groceryList in groceryLists)
        {
            groceryList.Delete();
        }
        foodItemRepository.UpdateRange(foodItems);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class GroceryListCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<GroceryList> groceryListRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<GroceryListCreatedDomainEvent>
{
    public async override Task Handle(GroceryListCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        GroceryList? groceryList = await groceryListRepository.GetByIdAsync(domainEvent.GroceryListId, cancellationToken);
        if (groceryList == null)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), GroceryListErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            groceryList.HouseholdId,
            ActivityCode.GROCERY_LIST_CREATE.ToString(),
            domainEvent.GroceryListId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(GroceryListCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class StorageCreatedEventHandler(
    IMilestoneService milestoneService,
    IActivityService activityService,
    IGenericRepository<Storage> storageRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<StorageCreatedDomainEvent>
{
    public async override Task Handle(StorageCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Storage? storage = await storageRepository.GetByIdAsync(domainEvent.StorageId, cancellationToken);
        if (storage == null)
        {
            throw new PentoException(nameof(StorageCreatedEventHandler), StorageErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            storage.HouseholdId,
            ActivityCode.STORAGE_CREATE.ToString(),
            domainEvent.StorageId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(StorageCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(StorageCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class CompartmentCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<CompartmentCreatedDomainEvent>
{
    public async override Task Handle(CompartmentCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Compartment? compartment = await compartmentRepository.GetByIdAsync(domainEvent.CompartmentId, cancellationToken);
        if (compartment == null)
        {
            throw new PentoException(nameof(CompartmentCreatedEventHandler), CompartmentErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            compartment.HouseholdId,
            ActivityCode.COMPARTMENT_CREATE.ToString(),
            domainEvent.CompartmentId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(CompartmentCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(CompartmentCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class RecipeCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<Recipe> compartmentRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<RecipeCreatedDomainEvent>
{
    public async override Task Handle(RecipeCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Recipe? recipe = await compartmentRepository.GetByIdAsync(domainEvent.RecipeId, cancellationToken);
        if (recipe == null)
        {
            throw new PentoException(nameof(RecipeCreatedEventHandler), RecipeErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            null,
            ActivityCode.RECIPE_CREATE.ToString(),
            domainEvent.RecipeId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(RecipeCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(RecipeCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
internal sealed class MealPlanCreatedEventHandler(
    IActivityService activityService,
    IMilestoneService milestoneService,
    IGenericRepository<MealPlan> mealPlanRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<MealPlanCreatedDomainEvent>
{
    public async override Task Handle(MealPlanCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        MealPlan? mealPlan = await mealPlanRepository.GetByIdAsync(domainEvent.MealPlanId, cancellationToken);
        if (mealPlan == null)
        {
            throw new PentoException(nameof(MealPlanCreatedEventHandler), MealPlanErrors.NotFound);
        }
        Result<UserActivity> createResult = await activityService.RecordActivityAsync(
            domainEvent.UserId,
            mealPlan.HouseholdId,
            ActivityCode.MEAL_PLAN_CREATE.ToString(),
            domainEvent.MealPlanId,
            cancellationToken);
        if (createResult.IsFailure)
        {
            throw new PentoException(nameof(MealPlanCreatedEventHandler), createResult.Error);
        }
        Result milestoneCheckResult = await milestoneService.CheckMilestoneAfterActivityAsync(createResult.Value, cancellationToken);
        if (milestoneCheckResult.IsFailure)
        {
            throw new PentoException(nameof(MealPlanCreatedEventHandler), milestoneCheckResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
