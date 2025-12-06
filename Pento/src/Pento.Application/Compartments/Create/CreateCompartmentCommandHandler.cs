using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.Storages;

namespace Pento.Application.Compartments.Create;

internal sealed class CreateCompartmentCommandHandler(
    IUserContext userContext,
    IGenericRepository<Compartment> compartmentRepository,
    IGenericRepository<Storage> storageRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCompartmentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCompartmentCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(StorageErrors.ForbiddenAccess);
        }
        Storage? storage = await storageRepository.GetByIdAsync(command.StorageId, cancellationToken);
        if (storage is null)
        {
            return Result.Failure<Guid>(StorageErrors.NotFound);
        }
        var compartment = Compartment.Create(
            command.Name,
            command.StorageId,
            householdId.Value,
            command.Notes,
            userContext.UserId
        );
        compartmentRepository.Add(compartment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return compartment.Id;

    }
}
