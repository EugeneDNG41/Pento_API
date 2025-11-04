using JasperFx.Events.Daemon;
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
using Pento.Domain.PossibleUnits;
using Pento.Domain.Storages;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Application.FoodItems.Create;
#pragma warning disable CS9113 // Parameter is unread.
#pragma warning disable S125
internal sealed class CreateFoodItemCommandHandler(
    IUserContext userContext,
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<FoodReference> foodReferenceRepository,
    IGenericRepository<Unit> unitRepository,
    IGenericRepository<PossibleUnit> possibleUnitRepository,
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
            //PossibleUnit? defaultPossibleUnit = (await possibleUnitRepository.FindAsync(pu => pu.FoodRefId == foodReference.Id && pu.IsDefault, cancellationToken)).SingleOrDefault();
            //if (defaultPossibleUnit is null)
            //{
            //    return Result.Failure<Guid>(PossibleUnitErrors.NoDefaultUnit);
            //}
            //Unit defaultUnit = await unitRepository.GetByIdAsync(defaultPossibleUnit.UnitId, cancellationToken);
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
            else if (await possibleUnitRepository.AnyAsync(pu => pu.UnitId == command.UnitId && pu.FoodRefId == foodReference.Id, cancellationToken))
            {
                validUnit = commandUnit;
            }
            else
            {
                return Result.Failure<Guid>(FoodItemErrors.InvalidMeasurementUnit);
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
        DateTime validExpirationDate;
        if (command.ExpirationDate is null)
        {
            Storage? storage = await storageRepository.GetByIdAsync(compartment.StorageId, cancellationToken);
            if (storage is null)
            {
                return Result.Failure<Guid>(StorageErrors.NotFound);
            }
            validExpirationDate = dateTimeProvider.UtcNow.AddDays(storage.Type switch
            {
                StorageType.Pantry => foodReference.TypicalShelfLifeDays_Pantry,
                StorageType.Fridge => foodReference.TypicalShelfLifeDays_Fridge,
                StorageType.Freezer => foodReference.TypicalShelfLifeDays_Freezer,
                _ => 5
            });
        }
        else
        {
            validExpirationDate = command.ExpirationDate.Value.ToUniversalTime();
        }
        var e = new FoodItemAdded(
                Guid.CreateVersion7(),
                command.FoodReferenceId,
                command.CompartmentId,
                compartment.Name,
                householdId.Value,
                command.Name is null ? foodReference.Name : command.Name,
                foodReference.ImageUrl,
                command.Quantity,
                validUnit.Abbreviation,
                validUnit.Id,
                validExpirationDate,
                command.Notes,
                null);
        //var e = new FoodItemAdded(
        //        Guid.CreateVersion7(),
        //        command.FoodRefId,
        //        command.CompartmentId,
        //        "default-compartment",
        //        householdId.Value,
        //        command.Name!,
        //        new Uri("https://www.somewebsite.org/books/RestInPractice.pdf"),
        //        command.Quantity,
        //        validUnit.Abbreviation,
        //        validUnit.Id,
        //        command.ExpirationDate!.Value,
        //        command.Notes,
        //        null);

        session.Events.StartStream<FoodItem>(e.Id, e);
        session.LastModifiedBy = userContext.UserId.ToString();
        await session.SaveChangesAsync(cancellationToken);
        return e.Id;
    }
}
