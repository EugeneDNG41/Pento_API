using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.GroceryLists;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Storages;
using Pento.Domain.Trades;

namespace Pento.Application.EventHandlers.Households;

internal sealed class HouseholdDeletedEventHandler(
    IGenericRepository<FoodItemReservation> foodItemReservationRepository,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<MealPlan> mealPlanRepository,
    IGenericRepository<GroceryList> groceryListRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IUnitOfWork unitOfWork) : DomainEventHandler<HouseholdDeletedDomainEvent>
{
    public async override Task Handle(HouseholdDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        IEnumerable<Storage> storages = await storageRepository.FindAsync(s => s.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        await storageRepository.RemoveRangeAsync(storages, cancellationToken);

        IEnumerable<FoodItemReservation> foodItemReservations = await foodItemReservationRepository.FindAsync(fir => fir.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        await foodItemReservationRepository.RemoveRangeAsync(foodItemReservations, cancellationToken);

        IEnumerable<MealPlan> mealPlans = await mealPlanRepository.FindAsync(mp => mp.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        await mealPlanRepository.RemoveRangeAsync(mealPlans, cancellationToken);

        IEnumerable<GroceryList> groceryLists = await groceryListRepository.FindAsync(gl => gl.HouseholdId == domainEvent.HouseholdId, cancellationToken);
        await groceryListRepository.RemoveRangeAsync(groceryLists, cancellationToken);

        IEnumerable<TradeOffer> openOffers = await tradeOfferRepository.FindAsync(
            offer => offer.HouseholdId == domainEvent.HouseholdId,
            cancellationToken);
        await tradeOfferRepository.RemoveRangeAsync(openOffers, cancellationToken);

        IEnumerable<TradeRequest> pendingRequests = await tradeRequestRepository.FindAsync(
            request => request.HouseholdId == domainEvent.HouseholdId,
            cancellationToken);
        await tradeRequestRepository.RemoveRangeAsync(pendingRequests,cancellationToken);

        IEnumerable<TradeSession> ongoingSessions = await tradeSessionRepository.FindAsync(
            session => (session.OfferHouseholdId == domainEvent.HouseholdId ||
                       session.RequestHouseholdId == domainEvent.HouseholdId)
            && session.Status == TradeSessionStatus.Ongoing,
            cancellationToken);
        foreach (TradeSession session in ongoingSessions)
        {
            session.Cancel(); 
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
