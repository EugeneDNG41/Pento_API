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

namespace Pento.Application.Trades.TradeItem.Requests.Create;

internal sealed class CreateTradeItemRequestCommandHandler(
    IUserContext userContext,
    IGenericRepository<TradeRequest> requestRepo,
    IGenericRepository<TradeItemRequest> itemRepo,
    IGenericRepository<TradeOffer> offerRepo,
    IGenericRepository<FoodItemDonationReservation> reservationRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IGenericRepository<Unit> unitRepo,
    IConverterService converter,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<CreateTradeItemRequestCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTradeItemRequestCommand command,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        Guid? householdId = userContext.HouseholdId;

        if (userId == Guid.Empty)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }

        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        TradeOffer? offer = await offerRepo.GetByIdAsync(command.TradeOfferId, cancellationToken);
        if (offer is null)
        {
            return Result.Failure<Guid>(TradeErrors.OfferNotFound);
        }

        TradeRequest? req = (await requestRepo.FindAsync(
            r => r.TradeOfferId == command.TradeOfferId && r.UserId == userId,
            cancellationToken)).FirstOrDefault();

        if (req is not null && req.Status != TradeRequestStatus.Rejected)
        {
            return Result.Failure<Guid>(TradeErrors.DuplicateRequest);
        }
        else
        {
            req = TradeRequest.Create(userId, command.TradeOfferId);
            requestRepo.Add(req);
        }

        foreach (CreateTradeItemRequestDto dto in command.Items)
        {
            FoodItem? foodItem = await foodItemRepo.GetByIdAsync(dto.FoodItemId, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure<Guid>(FoodItemErrors.NotFound);
            }

            if (foodItem.HouseholdId != householdId.Value)
            {
                return Result.Failure<Guid>(FoodItemErrors.ForbiddenAccess);
            }

            Unit? unit = await unitRepo.GetByIdAsync(dto.UnitId, cancellationToken);
            if (unit is null)
            {
                return Result.Failure<Guid>(UnitErrors.NotFound);
            }

            decimal qtyInItemUnit = dto.Quantity;
            if (dto.UnitId != foodItem.UnitId)
            {
                Result<decimal> conv = await converter.ConvertAsync(
                    dto.Quantity, dto.UnitId, foodItem.UnitId, cancellationToken);

                if (conv.IsFailure)
                {
                    return Result.Failure<Guid>(conv.Error);
                }

                qtyInItemUnit = conv.Value;
            }

            if (qtyInItemUnit > foodItem.Quantity)
            {
                return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
            }

            var item = TradeItemRequest.Create(
                foodItemId: dto.FoodItemId,
                quantity: dto.Quantity,
                unitId: dto.UnitId,
                requestId: req.Id
            );

            itemRepo.Add(item);

            var reservation = new FoodItemDonationReservation(
                id: Guid.CreateVersion7(),
                foodItemId: foodItem.Id,
                householdId: householdId.Value,
                reservationDateUtc: clock.UtcNow,
                quantity: dto.Quantity,
                unitId: dto.UnitId,
                reservationStatus: ReservationStatus.Pending,
                reservationFor: ReservationFor.Donation,
                giveAwayPostId: req.Id
            );

            reservationRepo.Add(reservation);

            foodItem.Reserve(
                qtyInItemUnit,
                dto.Quantity,
                dto.UnitId,
                userId
            );
        }

        await uow.SaveChangesAsync(cancellationToken);
        return req.Id;
    }
}
