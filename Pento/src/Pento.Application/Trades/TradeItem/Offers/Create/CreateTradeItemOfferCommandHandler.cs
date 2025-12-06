using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Application.Trades.TradeItem.Offers.Create;

internal sealed class CreateTradeItemOfferCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeOffer> offerRepository,
    IGenericRepository<TradeItemOffer> itemRepository,
    IGenericRepository<FoodItemDonationReservation> reservationRepository,
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Unit> unitRepository,
    IConverterService converter,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<CreateTradeItemOfferCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTradeItemOfferCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;


        if (userId == Guid.Empty)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        if (userContext.HouseholdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        Guid householdId = userContext.HouseholdId.Value;
        var offer = TradeOffer.Create(
            userId: userId,
            startDate: command.StartDate,
            endDate: command.EndDate,
            pickupOption: command.PickupOption,
            createOn: clock.UtcNow
        );

        offerRepository.Add(offer);

        foreach (CreateTradeItemOfferDto dto in command.Items)
        {
            FoodItem? foodItem =
                await foodItemRepository.GetByIdAsync(dto.FoodItemId, cancellationToken);

            if (foodItem is null)
            {
                return Result.Failure<Guid>(FoodItemErrors.NotFound);
            }

            if (foodItem.HouseholdId != householdId)
            {
                return Result.Failure<Guid>(FoodItemErrors.ForbiddenAccess);
            }

            Unit? fromUnit = await unitRepository.GetByIdAsync(dto.UnitId, cancellationToken);
            if (fromUnit is null)
            {
                return Result.Failure<Guid>(UnitErrors.NotFound);
            }

            decimal qtyInItemUnit = dto.Quantity;

            if (dto.UnitId != foodItem.UnitId)
            {
                Result<decimal> converted = await converter.ConvertAsync(
                    dto.Quantity,
                    dto.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );

                if (converted.IsFailure)
                {
                    return Result.Failure<Guid>(converted.Error);
                }

                qtyInItemUnit = converted.Value;
            }

            if (qtyInItemUnit > foodItem.Quantity)
            {
                return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
            }

            var item = TradeItemOffer.Create(
                foodItemId: dto.FoodItemId,
                quantity: dto.Quantity,
                unitId: dto.UnitId,
                offerId: offer.Id
            );

            itemRepository.Add(item);

            var reservation = new FoodItemDonationReservation(
                id: Guid.CreateVersion7(),
                foodItemId: foodItem.Id,
                householdId: householdId,
                reservationDateUtc: clock.UtcNow,
                quantity: dto.Quantity,
                unitId: dto.UnitId,
                reservationStatus: ReservationStatus.Pending,
                reservationFor: ReservationFor.Donation,
                giveAwayPostId: offer.Id  
            );

            reservationRepository.Add(reservation);

            foodItem.Reserve(
                qtyInItemUnit,
                dto.Quantity,
                dto.UnitId,
                userId
            );
        }

        await uow.SaveChangesAsync(cancellationToken);
        return offer.Id;
    }
}

