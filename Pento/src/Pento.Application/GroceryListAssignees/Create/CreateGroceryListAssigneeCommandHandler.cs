using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListAssignees;
using Pento.Domain.GroceryLists;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.GroceryListAssignees.Create;

internal sealed class CreateGroceryListAssigneeCommandHandler(
    IGenericRepository<GroceryListAssignee> groceryListAssigneeRepository,
    IGenericRepository<GroceryList> groceryListdRepository,
    IGenericRepository<User> userdRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateGroceryListAssigneeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGroceryListAssigneeCommand request, CancellationToken cancellationToken)
    {
        bool exists = await groceryListAssigneeRepository.AnyAsync(
            x => x.GroceryListId == request.GroceryListId && x.HouseholdMemberId == request.HouseholdMemberId,
            cancellationToken);

        if (exists)
        {
            return Result.Failure<Guid>(GroceryListAssigneeErrors.DuplicateAssignment);
        } 

        GroceryList? groceryList = await groceryListdRepository.GetByIdAsync(request.GroceryListId, cancellationToken);
        if (groceryList == null || groceryList.Id != request.GroceryListId)
        {
            return Result.Failure<Guid>(GroceryListAssigneeErrors.NotFoundByList(request.GroceryListId));
        }

        User? householdMember = await userdRepository.GetByIdAsync(request.HouseholdMemberId, cancellationToken);
        if (householdMember == null || householdMember.Id != request.HouseholdMemberId)
        {
            return Result.Failure<Guid>(GroceryListAssigneeErrors.NotFoundByMember);
        }
        var assignee = new GroceryListAssignee(
            id: Guid.CreateVersion7(),
            groceryListId: request.GroceryListId,
            householdMemberId: request.HouseholdMemberId,
            assignedOnUtc: dateTimeProvider.UtcNow
        );

        groceryListAssigneeRepository.Add(assignee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(assignee.Id);
    }
}
