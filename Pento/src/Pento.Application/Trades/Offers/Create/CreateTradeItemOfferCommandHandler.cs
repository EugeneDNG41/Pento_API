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

namespace Pento.Application.Trades.Offers.Create;

internal sealed class CreateTradeItemOfferCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeOffer> offerRepository,
    IGenericRepository<TradeItemOffer> tradeItemRepository,
    IGenericRepository<FoodItem> foodItemRepository,
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
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }    
        var offer = TradeOffer.Create(
            userId: userId,
            householdId: householdId.Value,
            startDate: command.StartDate,
            endDate: command.EndDate,
            pickupOption: command.PickupOption,
            createdOn: clock.UtcNow
        );

        offerRepository.Add(offer);

        foreach (AddTradeItemDto dto in command.Items)
        {
            FoodItem? foodItem =await foodItemRepository.GetByIdAsync(dto.FoodItemId, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure<Guid>(FoodItemErrors.NotFound);
            }

            if (foodItem.HouseholdId != householdId)
            {
                return Result.Failure<Guid>(FoodItemErrors.ForbiddenAccess);
            }
            Result<decimal> qtyInItemUnit = await converter.ConvertAsync(
                    dto.Quantity,
                    dto.UnitId,
                    foodItem.UnitId,
                    cancellationToken
                );
            if (qtyInItemUnit.IsFailure)
            {
                return Result.Failure<Guid>(qtyInItemUnit.Error);
            }         
            if (qtyInItemUnit.Value > foodItem.Quantity)
            {
                return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
            }
            var item = TradeItemOffer.Create(
                foodItemId: dto.FoodItemId,
                quantity: dto.Quantity,
                unitId: dto.UnitId,
                offerId: offer.Id
            );
            tradeItemRepository.Add(item);
            foodItem.Reserve(qtyInItemUnit.Value, dto.Quantity, dto.UnitId, userId);
            foodItemRepository.Update(foodItem);
        }

        await uow.SaveChangesAsync(cancellationToken);
        return offer.Id;
    }
}

