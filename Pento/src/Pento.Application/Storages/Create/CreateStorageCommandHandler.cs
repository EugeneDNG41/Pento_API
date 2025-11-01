using JasperFx.Events.Daemon;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Storages;
using Pento.Domain.Users;

namespace Pento.Application.Storages.Create;

internal sealed class CreateStorageCommandHandler(
    IUserContext userContext,
    IGenericRepository<Storage> storageRepository, 
    IGenericRepository<Household> householdReposiry,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateStorageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateStorageCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(UserErrors.NotInAnyHouseHold);
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
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return storage.Id;
    }
}
