using JasperFx.Events.Daemon;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Storages;
using Pento.Domain.Users;

namespace Pento.Application.Storages.Create;

internal sealed class CreateStorageCommandHandler(
    IGenericRepository<Storage> storageRepository, 
    IGenericRepository<Household> householdReposiry,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateStorageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateStorageCommand command, CancellationToken cancellationToken)
    {
        if (command.HouseholdId is null)
        {
            return Result.Failure<Guid>(UserErrors.NotInAnyHouseHold);
        }
        Household? household = await householdReposiry.GetByIdAsync(command.HouseholdId.Value, cancellationToken);
        if (household is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotFound);
        }
        var storage = Storage.Create(
            command.Name,
            command.HouseholdId.Value,
            command.type,
            command.notes);
        storageRepository.Add(storage);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return storage.Id;
    }
}
