
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.Households;
using Pento.Domain.Storages;


namespace Pento.Application.Storages.Create;

internal sealed class CreateStorageCommandHandler(
    IUserContext userContext,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<Household> householdReposiry,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateStorageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateStorageCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }
        Household? household = await householdReposiry.GetByIdAsync(householdId.Value, cancellationToken);
        if (household is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotFound);
        }
        var storage = Storage.Create(
            command.Name,
            householdId.Value,
            command.Type,
            command.Notes);
        storageRepository.Add(storage);
        var compartment = Compartment.Create(
            "Default",
            storage.Id,
            householdId.Value,
            null);
        compartmentRepository.Add(compartment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return storage.Id;
    }
}
