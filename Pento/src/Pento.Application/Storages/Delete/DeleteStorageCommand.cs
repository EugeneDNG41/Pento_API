using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Delete;

public sealed record DeleteStorageCommand(Guid StorageId) : ICommand;

internal sealed class DeleteStorageCommandValidator : AbstractValidator<DeleteStorageCommand>
{
    public DeleteStorageCommandValidator()
    {
        RuleFor(x => x.StorageId)
            .NotEmpty().WithMessage("Storage ID must not be empty.");
    }
}
internal sealed class DeleteStorageCommandHandler(
    IUserContext userContext,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteStorageCommand>
{
    public async Task<Result> Handle(DeleteStorageCommand request, CancellationToken cancellationToken)
    {
        Guid? currentHouseholdId = userContext.HouseholdId;
        Storage? storage = await storageRepository.GetByIdAsync(request.StorageId, cancellationToken);
        if (storage is null)
        {
            return Result.Failure(StorageErrors.NotFound);
        }
        if (storage.HouseholdId != currentHouseholdId)
        {
            return Result.Failure(StorageErrors.ForbiddenAccess);
        }
        if (await compartmentRepository.AnyAsync(c => c.StorageId == storage.Id, cancellationToken))
        {
            return Result.Failure(StorageErrors.NotEmpty);
        }
        storage.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
