using Marten;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodItems.Events;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Application.FoodItems.Create;

internal sealed class CreateFoodItemCommandHandler(
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Unit> unitRepository,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<Storage> storageRepository,
    IDocumentSession session)
    : ICommandHandler<CreateFoodItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFoodItemCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }
        FoodReference? foodReference = await foodReferenceRepository.GetByIdAsync(command.FoodReferenceId, cancellationToken);
        if (foodReference is null)
        {
            return Result.Failure<Guid>(FoodReferenceErrors.NotFound);
        }
        Unit validUnit;
        if (command.UnitId is null)
        {
            Unit defaultUnit = (await unitRepository.FindAsync(u => u.ToBaseFactor == 1, cancellationToken)).First();
            validUnit = defaultUnit;
        }
        else
        {
            Unit? commandUnit = await unitRepository.GetByIdAsync(command.UnitId.Value, cancellationToken);
            if (commandUnit is null)
            {
                return Result.Failure<Guid>(UnitErrors.NotFound);
            }
            else
            {
                validUnit = commandUnit;
            }
        }
        Compartment? compartment = await compartmentRepository.GetByIdAsync(command.CompartmentId, cancellationToken);
        if (compartment is null)
        {
            return Result.Failure<Guid>(CompartmentErrors.NotFound);
        }
        if (compartment.HouseholdId != householdId.Value)
        {
            return Result.Failure<Guid>(CompartmentErrors.ForbiddenAccess);
        }
        Storage? storage = await storageRepository.GetByIdAsync(compartment.StorageId, cancellationToken);
        if (storage is null)
        {
            return Result.Failure<Guid>(StorageErrors.NotFound);
        }
        DateTime validExpirationDate = command.ExpirationDate is null
            ? dateTimeProvider.UtcNow.AddDays(storage.Type switch
            {
                StorageType.Pantry => foodReference.TypicalShelfLifeDays_Pantry,
                StorageType.Fridge => foodReference.TypicalShelfLifeDays_Fridge,
                StorageType.Freezer => foodReference.TypicalShelfLifeDays_Freezer,
                _ => 5
            })
            : command.ExpirationDate.Value.ToUniversalTime();

        var e = new FoodItemAdded(
                Guid.CreateVersion7(),
                command.FoodReferenceId,
                command.CompartmentId,
                householdId.Value,
                command.Name is null ? foodReference.Name : command.Name,
                foodReference.ImageUrl,
                command.Quantity,
                validUnit.Id,
                validExpirationDate,
                command.Notes,
                null);

        session.Events.StartStream<FoodItem>(e.Id, e);
        session.LastModifiedBy = userContext.UserId.ToString();
        await session.SaveChangesAsync(cancellationToken);
        return e.Id;
    }
}
