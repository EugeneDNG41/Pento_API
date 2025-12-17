using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
using Pento.Application.GroceryListItems.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.GroceryListItems;
using Pento.Domain.GroceryLists;
using Pento.Domain.Units;
namespace Pento.Application.GroceryListItems.Create;
internal sealed class CreateGroceryListItemCommandHandler(
    IGenericRepository<GroceryListItem> _groceryListItemRepository,
    IGenericRepository<FoodReference> _foodrefRepo,
    IGenericRepository<GroceryList> _grocerylistRepository,
    IGenericRepository<Unit> _unitrepository,
    IUserContext userContext,
    IUnitOfWork _unitOfWork,
    IDateTimeProvider _dateTimeProvider,
    IConverterService _converterService             
) : ICommandHandler<CreateGroceryListItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGroceryListItemCommand request, CancellationToken cancellationToken)
    {
        if (userContext.UserId == Guid.Empty)
        {
            return Result.Failure<Guid>(GroceryListItemErrors.ForbiddenAccess);
        }

        GroceryList groceryList = await _grocerylistRepository.GetByIdAsync(request.ListId, cancellationToken);
        if (groceryList is null)
        {
            return Result.Failure<Guid>(GroceryListErrors.NotFound);
        }

        FoodReference foodReference = await _foodrefRepo.GetByIdAsync(request.FoodRefId, cancellationToken);
        if (foodReference is null)
        {
            return Result.Failure<Guid>(FoodReferenceErrors.NotFound);
        }

        Unit? requestUnit = request.UnitId is null
            ? null
            : await _unitrepository.GetByIdAsync(request.UnitId.Value, cancellationToken);

        if (request.UnitId is not null && requestUnit is null)
        {
            return Result.Failure<Guid>(UnitErrors.NotFound);
        }

        if (!Enum.TryParse<GroceryItemPriority>(request.Priority, true, out GroceryItemPriority priority))
        {
            return Result.Failure<Guid>(GroceryListItemErrors.InvalidPriority);
        }

        IEnumerable<GroceryListItem> existingItems = await _groceryListItemRepository.FindAsync(
            x => x.ListId == request.ListId &&
                 x.FoodRefId == request.FoodRefId,
            cancellationToken
        );
        GroceryListItem? existingItem = existingItems.FirstOrDefault(x =>
        {
            bool sameUnit = (x.UnitId ?? Guid.Empty) == (request.UnitId ?? Guid.Empty);
            bool samePriority = x.Priority == priority;

            bool sameNotes =
                string.IsNullOrWhiteSpace(x.Notes) && string.IsNullOrWhiteSpace(request.Notes)
                || x.Notes == request.Notes;

            return sameUnit && samePriority && sameNotes;
        });



        if (existingItem is not null)
        {

            Guid existingUnitId = existingItem.UnitId ?? Guid.Empty;
            Guid requestUnitId = request.UnitId ?? Guid.Empty;

            Result<decimal> converted = await _converterService.ConvertAsync(
                request.Quantity,
                requestUnitId,
                existingUnitId,
                cancellationToken
            );

            if (converted.IsFailure)
            {
                return Result.Failure<Guid>(converted.Error);
            }

            decimal quantityToAdd = converted.Value;

            existingItem.IncreaseQuantity(quantityToAdd);

            await _groceryListItemRepository.UpdateAsync(existingItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return existingItem.Id;
        }

        var groceryListItem = new GroceryListItem(
            id: Guid.NewGuid(),
            listId: request.ListId,
            foodRefId: request.FoodRefId,
            quantity: request.Quantity,
            addedBy: userContext.UserId,
            createdOnUtc: _dateTimeProvider.UtcNow,
            customName: request.CustomName,
            unitId: request.UnitId,
            notes: request.Notes,
            priority: priority
        );

        _groceryListItemRepository.Add(groceryListItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return groceryListItem.Id;
    }
}
