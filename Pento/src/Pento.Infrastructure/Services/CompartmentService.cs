using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Trades;

namespace Pento.Infrastructure.Services;

internal sealed class CompartmentService(
    IGenericRepository<FoodItemReservation> foodItemReservationRepository,
    IGenericRepository<TradeOffer> tradeOfferRepository,
    IGenericRepository<TradeRequest> tradeRequestRepository,
    IGenericRepository<TradeSession> tradeSessionRepository,
    IGenericRepository<TradeItemOffer> tradeItemOfferRepository,
    IGenericRepository<TradeItemRequest> tradeItemRequestRepository,
    IGenericRepository<TradeSessionItem> tradeSessionItemRepository,
    IGenericRepository<FoodItem> foodItemRepository) : ICompartmentService
{
    public async Task<Result> CheckIfEmptyAsync(Guid compartmentId, Guid householdId, CancellationToken cancellationToken)
    {
        bool itemsInCompartment = await foodItemRepository.AnyAsync(item => item.CompartmentId == compartmentId && item.Quantity > 0, cancellationToken);
        if (itemsInCompartment)
        {
            return Result.Failure(CompartmentErrors.NotEmpty);
        }
        var reservedFoodIds = (await foodItemReservationRepository
            .FindAsync(r => r.HouseholdId == householdId && r.Status == ReservationStatus.Pending, cancellationToken))
            .Select(r => r.FoodItemId).ToList();

        var openOfferIds = (await tradeOfferRepository
            .FindAsync(to => to.HouseholdId == householdId && to.Status == TradeOfferStatus.Open, cancellationToken))
            .Select(to => to.Id).ToList();
        var offeredFoodIds = (await tradeItemOfferRepository
            .FindAsync(tio => openOfferIds.Contains(tio.OfferId), cancellationToken))
            .Select(ti => ti.FoodItemId).ToList();

        var pendingRequestIds = (await tradeRequestRepository
            .FindAsync(tr => tr.HouseholdId == householdId && tr.Status == TradeRequestStatus.Pending, cancellationToken))
            .Select(tr => tr.Id).ToList();
        var requestedFoodIds = (await tradeItemRequestRepository
            .FindAsync(tir => pendingRequestIds.Contains(tir.RequestId), cancellationToken))
            .Select(ti => ti.FoodItemId).ToList();

        var ongoingSessionIds = (await tradeSessionRepository
            .FindAsync(ts => (ts.OfferHouseholdId == householdId || ts.RequestHouseholdId == householdId) && ts.Status == TradeSessionStatus.Ongoing, cancellationToken))
            .Select(ts => ts.Id).ToList();
        var tradeSessionItemFoodIds = (await tradeSessionItemRepository
            .FindAsync(tis => ongoingSessionIds.Contains(tis.SessionId), cancellationToken))
            .Select(ti => ti.FoodItemId).ToList();

        var foodIds = reservedFoodIds
            .Concat(offeredFoodIds)
            .Concat(requestedFoodIds)
            .Concat(tradeSessionItemFoodIds)
            .Distinct()
            .ToList();
        bool hasReservationsOrTrades =
            await foodItemRepository.AnyAsync(
                fi => fi.CompartmentId == compartmentId
                   && foodIds.Contains(fi.Id),
                cancellationToken);
        if (hasReservationsOrTrades)
        {
            return Result.Failure(CompartmentErrors.ItemsInUse);
        }
        return Result.Success();
    }
}
