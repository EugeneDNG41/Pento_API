
using Microsoft.AspNetCore.SignalR;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;

namespace Pento.Application.FoodItems.Discard;

internal sealed class DiscardFoodItemCommandHandler(
    IConverterService converter,
    IUserContext userContext,
    IGenericRepository<FoodItem> foodItemRepository,
    IHubContext<MessageHub, IMessageClient> hubContext,
    IUnitOfWork unitOfWork) : ICommandHandler<DiscardFoodItemCommand>
{
    public async Task<Result> Handle(DiscardFoodItemCommand command, CancellationToken cancellationToken)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.Id, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(FoodItemErrors.ForbiddenAccess);
        }
        decimal requestedQtyInItemUnit = command.Quantity;
        if (foodItem.UnitId != command.UnitId)
        {
            Result<decimal> convertedResult = await converter.ConvertAsync(
                command.Quantity,
                fromUnitId: command.UnitId,
                toUnitId: foodItem.UnitId,
                cancellationToken);
            if (convertedResult.IsFailure)
            {
                return Result.Failure<Guid>(convertedResult.Error);
            }
            requestedQtyInItemUnit = convertedResult.Value;
        }
        if (requestedQtyInItemUnit > foodItem.Quantity)
        {
            return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
        }
        foodItem.Discard(requestedQtyInItemUnit, command.Quantity, command.UnitId, userContext.UserId);
        await foodItemRepository.UpdateAsync(foodItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await hubContext.Clients.Group(userContext.HouseholdId.Value.ToString())
            .FoodItemQuantityUpdated(foodItem.Id, foodItem.Quantity);
        return Result.Success();
    }
}
