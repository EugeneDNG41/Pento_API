using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListItems;

namespace Pento.Application.GroceryListItems.Update;

internal sealed class UpdateGroceryListItemCommandHandler(
    IGenericRepository<GroceryListItem> groceryListItemRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateGroceryListItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateGroceryListItemCommand request, CancellationToken cancellationToken)
    {
        GroceryListItem? groceryListItem = await groceryListItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (groceryListItem is null)
        {
            return Result.Failure<Guid>(GroceryListItemErrors.NotFound);
        }

        if (!Enum.TryParse<GroceryItemPriority>(request.Priority, true, out GroceryItemPriority priority))
        {
            return Result.Failure<Guid>(GroceryListItemErrors.InvalidPriority);
        }

        groceryListItem.Update(
            quantity: request.Quantity,
            notes: request.Notes,
            customName: request.CustomName,
            priority: priority
        );

        await groceryListItemRepository.UpdateAsync(groceryListItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(request.Id);
    }
}
