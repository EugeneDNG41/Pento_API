using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryLists;

namespace Pento.Application.GroceryLists.Delete;

internal sealed class DeleteGroceryListCommandHandler(
    IUserContext userContext,
    IGenericRepository<GroceryList> groceryListRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteGroceryListCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteGroceryListCommand command, CancellationToken cancellationToken)
    {
        GroceryList? list = await groceryListRepository.GetByIdAsync(command.Id, cancellationToken);
        if (list is null)
        {
            return Result.Failure<Guid>(GroceryListErrors.NotFound);
        }

        if (list.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure<Guid>(GroceryListErrors.ForbiddenAccess);
        }

        groceryListRepository.Remove(list);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(list.Id);
    }
}
