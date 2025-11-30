using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;

namespace Pento.Application.FoodItems.Consume;

internal sealed class ConsumeFoodItemCommandHandler(
    IConverterService converter,
    IUserContext userContext,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ConsumeFoodItemCommand>
{
    public async Task<Result> Handle(ConsumeFoodItemCommand command, CancellationToken cancellationToken)
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
        foodItem.Consume(requestedQtyInItemUnit, command.Quantity, command.UnitId, userContext.UserId);
        foodItemRepository.Update(foodItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
