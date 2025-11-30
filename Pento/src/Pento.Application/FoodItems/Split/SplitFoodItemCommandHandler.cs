using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.Split;

internal sealed class SplitFoodItemCommandHandler(
    IConverterService converterService,
    IUserContext userContext,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SplitFoodItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(SplitFoodItemCommand command, CancellationToken cancellationToken)
    {
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.Id, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<Guid>(FoodItemErrors.ForbiddenAccess);
        }
        decimal requestedQtyInItemUnit = command.Quantity;
        if (foodItem.UnitId != command.UnitId)
        {
            Result<decimal> convertedResult = await converterService.ConvertAsync(
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
        if (requestedQtyInItemUnit >= foodItem.Quantity)
        {
            return Result.Failure<Guid>(FoodItemErrors.InsufficientQuantity);
        }
        var newFoodItem = FoodItem.Create(
            foodReferenceId: foodItem.FoodReferenceId,
            compartmentId: foodItem.CompartmentId,
            householdId: foodItem.HouseholdId,
            name: foodItem.Name,
            imageUrl: foodItem.ImageUrl,
            quantity: command.Quantity,
            unitId: command.UnitId,
            expirationDate: foodItem.ExpirationDate,
            notes: foodItem.Notes,
            addedBy: userContext.UserId);
        foodItemRepository.Add(newFoodItem);
        foodItem.AdjustQuantity(
            foodItem.Quantity - requestedQtyInItemUnit,
            userContext.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return newFoodItem.Id;
    }
}
