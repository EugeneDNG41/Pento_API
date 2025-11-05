using Marten;
using Marten.Events;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.Update;

internal sealed class UpdateFoodItemCommandHandler(
    IUserContext userContext,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Unit> unitRepository,
    IUnitConverter converter,
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
            Unit unit = await unitRepository.GetByIdAsync(command.UnitId, cancellationToken);
            foodItemEvents.Add(new FoodItemUnitChanged(command.UnitId, convertedResult.Value, unit!.Abbreviation));
        }
        //Move compartment
        if (foodItem.CompartmentId != command.CompartmentId)
        {
            Compartment? newCompartment = await compartmentRepository.GetByIdAsync(command.CompartmentId, cancellationToken);
            if (newCompartment is null)
            {
                return Result.Failure(CompartmentErrors.NotFound);
            } else if (newCompartment.HouseholdId != userContext.HouseholdId)
            {
                return Result.Failure(CompartmentErrors.ForbiddenAccess);
            } else
            {
                foodItemEvents.Add(new FoodItemCompartmentMoved(command.CompartmentId, newCompartment.Name));
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
        //Change expiration date (override storage type change)
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
            await session.Events.AppendOptimistic(command.Id, foodItemEvents);
        }
        return Result.Success();
    }
}

