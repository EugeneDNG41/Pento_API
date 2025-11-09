using JasperFx.Events;
using Marten;
using Marten.Events;
using Microsoft.FSharp.Control;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.Update;

internal sealed class UpdateFoodItemCommandHandler(
    IUserContext userContext,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    ICalculator converter,
    IDocumentSession session) : ICommandHandler<UpdateFoodItemCommand>
{
    public async Task<Result> Handle(UpdateFoodItemCommand command, CancellationToken cancellationToken)
    {
        IEventStream<FoodItem> stream = await session.Events.FetchForWriting<FoodItem>(command.Id, command.Version, cancellationToken);
        FoodItem? foodItem = stream.Aggregate;
        if (foodItem is null)
        {
            return Result.Failure(FoodItemErrors.NotFound);
        }
        if (foodItem.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(FoodItemErrors.ForbiddenAccess);
        }
        var foodItemEvents = new List<FoodItemEvent>();
        DateTime expirationDateUtc = command.ExpirationDate.ToUniversalTime();
       
        //Change measurement unit
        if (foodItem.UnitId != command.UnitId)
        {
            Result<decimal> convertedResult = await converter.ConvertAsync(foodItem.Quantity, foodItem.UnitId, command.UnitId, cancellationToken);
            if (convertedResult.IsFailure)
            {
                return Result.Failure(convertedResult.Error);
            }
            foodItemEvents.Add(new FoodItemUnitChanged(command.UnitId, convertedResult.Value));
        }
        //Move compartment
        if (foodItem.CompartmentId != command.CompartmentId)
        {
            Compartment? oldCompartment = await compartmentRepository.GetByIdAsync(foodItem.CompartmentId, cancellationToken);
            Compartment? newCompartment = await compartmentRepository.GetByIdAsync(command.CompartmentId, cancellationToken);
            if (newCompartment is null || oldCompartment is null)
            {
                return Result.Failure(CompartmentErrors.NotFound);
            } 
            else if (newCompartment.HouseholdId != userContext.HouseholdId || oldCompartment.HouseholdId != userContext.HouseholdId)
            {
                return Result.Failure(CompartmentErrors.ForbiddenAccess);
            } 
            else if (oldCompartment.Id == newCompartment.Id)
            {
                foodItemEvents.Add(new FoodItemCompartmentMoved(command.CompartmentId));
            }
            else
            {
                Storage? oldStorage = await storageRepository.GetByIdAsync(oldCompartment.StorageId, cancellationToken);
                Storage? newStorage = await storageRepository.GetByIdAsync(newCompartment.StorageId, cancellationToken);
                if (newStorage is null || oldStorage is null)
                {
                    return Result.Failure(StorageErrors.NotFound);
                }
                else if (newStorage.HouseholdId != userContext.HouseholdId || oldStorage.HouseholdId != userContext.HouseholdId)
                {
                    return Result.Failure(StorageErrors.ForbiddenAccess);
                } 
                else if (oldStorage.Type != newStorage.Type && foodItem.ExpirationDateUtc == expirationDateUtc)
                {
                    //Storage changed, adjust expiration date based on new newStorage typical shelf life
                    FoodReference? foodReference = await foodReferenceRepository.GetByIdAsync(foodItem.FoodReferenceId, cancellationToken);
                    if (foodReference is null)
                    {
                        return Result.Failure(FoodReferenceErrors.NotFound);
                    }
                    int typicalShelfLifeDays = newStorage.Type switch
                    {
                        StorageType.Pantry => foodReference.TypicalShelfLifeDays_Pantry,
                        StorageType.Fridge => foodReference.TypicalShelfLifeDays_Fridge,
                        StorageType.Freezer => foodReference.TypicalShelfLifeDays_Freezer,
                        _ => 0
                    };
                    IEvent? lastMoveEvent = stream.Events
                            .Where(x => x.EventTypesAre(typeof(IEvent<FoodItemStorageTypeChanged>), typeof(FoodItemAdded)))
                            .OrderByDescending(x => x.Timestamp)
                            .FirstOrDefault();
                    if (lastMoveEvent is null)
                    {
                        expirationDateUtc = foodItem.ExpirationDateUtc.AddDays(typicalShelfLifeDays);
                    }
                    else
                    {
                        DateTime newExpirationDateUtc = converter.CalculateNewExpiryRemainingFraction(
                            lastPlacedAtUtc: lastMoveEvent.Timestamp.UtcDateTime,
                            oldType: oldStorage.Type,
                            newType: newStorage.Type,
                            foodRef: foodReference,
                            currentExpiryUtc: foodItem.ExpirationDateUtc
                        );
                        expirationDateUtc = newExpirationDateUtc;
                    }
                    foodItemEvents.Add(new FoodItemStorageMoved(newStorage.Id, newCompartment.Id));
                    foodItemEvents.Add(new FoodItemStorageTypeChanged(newStorage.Type));
                    foodItemEvents.Add(new FoodItemExpirationDateUpdated(expirationDateUtc));
                }
                else
                {
                    foodItemEvents.Add(new FoodItemStorageMoved(newStorage.Id, newCompartment.Id));
                }
                
            }
        }
        //Rename
        if (foodItem.Name != command.Name)
        {
            if (string.IsNullOrEmpty(command.Name))
            {
                FoodReference? foodReference = await foodReferenceRepository.GetByIdAsync(foodItem.FoodReferenceId, cancellationToken);
                if (foodReference is null)
                {
                    return Result.Failure(FoodReferenceErrors.NotFound);
                }
                foodItemEvents.Add(new FoodItemRenamed(foodReference.Name));
            }
            else
            {
                foodItemEvents.Add(new FoodItemRenamed(command.Name));
            }
        }
        //Change quantity (override converted quantity)
        if (foodItem.Quantity != command.Quantity)
        {
            foodItemEvents.Add(new FoodItemQuantityAdjusted(command.Quantity));
        }
        //Change expiration date (override newStorage type change)
        if (foodItem.ExpirationDateUtc != expirationDateUtc)
        {
            foodItemEvents.Add(new FoodItemExpirationDateUpdated(expirationDateUtc));
        }
        //Change notes
        if (foodItem.Notes != command.Notes)
        {
            foodItemEvents.Add(new FoodItemNotesUpdated(command.Notes));
        }

        if (!foodItemEvents.Any())
        {
            return Result.Success();
        } 
        else
        {
            session.LastModifiedBy = userContext.UserId.ToString();
            await session.Events.AppendOptimistic(command.Id, foodItemEvents);
            await session.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
}

