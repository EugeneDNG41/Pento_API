using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Storages;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Application.FoodItems.Update;

internal sealed class UpdateFoodItemCommandHandler(
    IUserContext userContext,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IConverterService converter,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateFoodItemCommand>
{
    public async Task<Result> Handle(UpdateFoodItemCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId == null)
        {
            return Result.Failure(HouseholdErrors.NotInAnyHouseHold);
        }
        FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.Id, cancellationToken);
        if (foodItem is null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != householdId)
        {
            return Result.Failure(FoodItemErrors.ForbiddenAccess);
        }      
        //Change measurement unit
        if (command.UnitId != null && foodItem.UnitId != command.UnitId)
        {
            Result<decimal> convertedResult = await converter.ConvertAsync(foodItem.Quantity, foodItem.UnitId, command.UnitId.Value, cancellationToken);
            if (convertedResult.IsFailure)
            {
                return Result.Failure(convertedResult.Error);
            }
            foodItem.ChangeUnit(command.UnitId.Value, convertedResult.Value, userContext.UserId);
        }
        //Move compartment
        if (command.CompartmentId != null && foodItem.CompartmentId != command.CompartmentId)
        {
            Compartment? oldCompartment = await compartmentRepository.GetByIdAsync(foodItem.CompartmentId, cancellationToken);
            Compartment? newCompartment = await compartmentRepository.GetByIdAsync(command.CompartmentId.Value, cancellationToken);
            if (newCompartment is null || oldCompartment is null)
            {
                return Result.Failure(CompartmentErrors.NotFound);
            } 
            else if (newCompartment.HouseholdId != householdId || oldCompartment.HouseholdId != householdId)
            {
                return Result.Failure(CompartmentErrors.ForbiddenAccess);
            }

            Storage? oldStorage = await storageRepository.GetByIdAsync(oldCompartment.StorageId, cancellationToken);
            Storage? newStorage = await storageRepository.GetByIdAsync(newCompartment.StorageId, cancellationToken);
            if (newStorage is null || oldStorage is null)
            {
                return Result.Failure(StorageErrors.NotFound);
            }
            else if (newStorage.HouseholdId != householdId || oldStorage.HouseholdId != householdId)
            {
                return Result.Failure(StorageErrors.ForbiddenAccess);
            }
            else if (oldStorage.Type != newStorage.Type && foodItem.ExpirationDate == command.ExpirationDate)
            {
                FoodReference? foodReference = await foodReferenceRepository.GetByIdAsync(foodItem.FoodReferenceId, cancellationToken);
                if (foodReference is null)
                {
                    return Result.Failure(FoodReferenceErrors.NotFound);
                }
                DateOnly newExpirationDateUtc = converter.CalculateNewExpiryRemainingFraction(
                        oldType: oldStorage.Type,
                        newType: newStorage.Type,
                        foodRef: foodReference,
                        currentExpiry: foodItem.ExpirationDate
                    );
                foodItem.AdjustExpirationDate(newExpirationDateUtc, userContext.UserId);
            }
            foodItem.MoveToCompartment(command.CompartmentId.Value, userContext.UserId);
        }
        //Rename
        if (!string.IsNullOrEmpty(command.Name) && foodItem.Name != command.Name)
        {
            foodItem.Rename(command.Name, userContext.UserId);
        }
        //Change quantity (override converted quantity)
        if (command.Quantity != null && foodItem.Quantity != command.Quantity)
        {
            foodItem.AdjustQuantity(command.Quantity.Value, userContext.UserId);
        }
        //Change expiration date (override newStorage type change)
        if (command.ExpirationDate != null && foodItem.ExpirationDate != command.ExpirationDate)
        {
            foodItem.AdjustExpirationDate(command.ExpirationDate.Value, userContext.UserId);
        }
        //Change notes
        if (foodItem.Notes != command.Notes)
        {
            foodItem.UpdateNotes(command.Notes, userContext.UserId);
        }
        foodItemRepository.Update(foodItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

