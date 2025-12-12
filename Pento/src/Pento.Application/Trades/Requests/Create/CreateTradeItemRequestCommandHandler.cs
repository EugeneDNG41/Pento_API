using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Requests.Create;

internal sealed class CreateTradeItemRequestCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
    IGenericRepository<TradeRequest> requestRepo,
    IGenericRepository<TradeItemRequest> tradeItemRepo,
    IGenericRepository<TradeOffer> offerRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IConverterService converter,
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
        if (offer.HouseholdId == householdId.Value)
        {
            return Result.Failure<Guid>(TradeErrors.CannotTradeWithinHousehold);
        }
        if (offer.Status != TradeOfferStatus.Open)
        {
            return Result.Failure<Guid>(TradeErrors.InvalidOfferState);
        }
        TradeRequest? request = (await requestRepo.FindAsync(
            r => r.TradeOfferId == command.TradeOfferId && r.HouseholdId == householdId.Value,
            cancellationToken)).FirstOrDefault();

        if (request is not null && request.Status == TradeRequestStatus.Pending)
        {
            return Result.Failure<Guid>(TradeErrors.DuplicateRequest);
        }
        else
        {
            request = TradeRequest.Create(userId, householdId.Value, command.TradeOfferId, dateTimeProvider.UtcNow);
            requestRepo.Add(request);
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
            Result<decimal> qtyInItemUnit = await converter.ConvertAsync(
                    dto.Quantity, dto.UnitId, foodItem.UnitId, cancellationToken);

            if (qtyInItemUnit.IsFailure)
            {
                return Result.Failure<Guid>(qtyInItemUnit.Error);
            }
            if (qtyInItemUnit.Value > foodItem.Quantity)
            {
                return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
            }
            var item = TradeItemRequest.Create(
                foodItemId: dto.FoodItemId,
                quantity: dto.Quantity,
                unitId: dto.UnitId,
                requestId: request.Id
            );

            tradeItemRepo.Add(item);
            foodItem.Reserve(qtyInItemUnit.Value, dto.Quantity, dto.UnitId, userId);
        }

        await uow.SaveChangesAsync(cancellationToken);
        return request.Id;
    }
}
