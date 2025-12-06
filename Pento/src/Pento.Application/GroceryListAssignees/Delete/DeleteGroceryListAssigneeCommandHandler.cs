using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListAssignees;

namespace Pento.Application.GroceryListAssignees.Delete;

internal sealed class DeleteGroceryListAssigneeCommandHandler(
    IGenericRepository<GroceryListAssignee> groceryListAssigneeRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteGroceryListAssigneeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteGroceryListAssigneeCommand request, CancellationToken cancellationToken)
    {
        GroceryListAssignee? assignee = await groceryListAssigneeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (assignee is null)
        {
            return Result.Failure<Guid>(GroceryListAssigneeErrors.NotFound);
        }

        groceryListAssigneeRepository.Remove(assignee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(request.Id);
    }
}
