using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListItems;

namespace Pento.Application.GroceryListItems.Delete;

internal sealed class DeleteGroceryListItemCommandHandler(
    IGenericRepository<GroceryListItem> groceryListItemRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteGroceryListItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteGroceryListItemCommand request, CancellationToken cancellationToken)
    {
        GroceryListItem? groceryListItem = await groceryListItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (groceryListItem is null)
        {
            return Result.Failure<Guid>(GroceryListItemErrors.NotFound);
        }

        groceryListItemRepository.Remove(groceryListItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(request.Id);
    }
}
