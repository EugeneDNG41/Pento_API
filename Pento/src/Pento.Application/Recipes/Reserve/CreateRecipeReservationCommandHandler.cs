using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Reserve;

internal sealed class CreateRecipeReservationCommandHandler(
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<Recipe> recipeRepository,
    IGenericRepository<FoodItemRecipeReservation> foodItemReservationRepository,
    IConverterService converter,
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CreateRecipeReservationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateRecipeReservationCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId == null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.FoodItemId, cancellationToken);
        if (foodItem == null)
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != householdId)
        {
            return Result.Failure<Guid>(FoodItemErrors.ForbiddenAccess);
        }
        Recipe? recipe = await recipeRepository.GetByIdAsync (command.RecipeId, cancellationToken);
        if (recipe == null)
        {
            return Result.Failure<Guid>(RecipeErrors.NotFound);
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
        var reservation = FoodItemRecipeReservation.Create(
            foodItemId: command.FoodItemId,
            householdId: householdId.Value,
            reservationDateUtc: dateTimeProvider.UtcNow,
            quantity: requestedQtyInItemUnit,
            unitId: command.UnitId,
            recipeId: command.RecipeId
            );

        foodItemReservationRepository.Add( reservation );
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return reservation.Id;
    }
}

