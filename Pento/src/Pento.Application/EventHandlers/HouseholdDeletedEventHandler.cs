using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.GroceryLists;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Storages;

namespace Pento.Application.EventHandlers;

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
