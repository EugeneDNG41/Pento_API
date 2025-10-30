using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Create;

internal sealed class CreateStorageCommandHandler(
    IGenericRepository<Storage> storageRepository, 
    IGenericRepository<Household> householdReposiry,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateStorageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateStorageCommand request, CancellationToken cancellationToken)
    {
        Household? household = await householdReposiry.GetByIdAsync(request.HouseholdId, cancellationToken);
        if (household is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotFound);
        }
        var storage = Storage.Create(
            request.Name,
            request.HouseholdId,
            request.type,
            request.notes);
        storageRepository.Add(storage);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return storage.Id;
    }
}
