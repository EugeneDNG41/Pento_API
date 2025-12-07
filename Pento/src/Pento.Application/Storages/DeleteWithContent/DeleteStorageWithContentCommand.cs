using FluentValidation;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.DeleteWithContent;

public sealed record DeleteStorageWithContentCommand(Guid StorageId) : ICommand;

internal sealed class DeleteStorageWithContentCommandValidator : AbstractValidator<DeleteStorageWithContentCommand>
{
    public DeleteStorageWithContentCommandValidator()
    {
        RuleFor(x => x.StorageId)
            .NotEmpty().WithMessage("Storage Id is required.");
    }
}
internal sealed class DeleteStorageWithContentCommandHandler(
    IUserContext userContext,
    IGenericRepository<Storage> storageRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteStorageWithContentCommand>
{
    public async Task<Result> Handle(DeleteStorageWithContentCommand request, CancellationToken cancellationToken)
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
        }// check if there's any reserved items
        storage.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
