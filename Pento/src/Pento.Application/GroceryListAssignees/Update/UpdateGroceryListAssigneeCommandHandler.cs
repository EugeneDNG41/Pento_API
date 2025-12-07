using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListAssignees;

namespace Pento.Application.GroceryListAssignees.Update;

internal sealed class UpdateGroceryListAssigneeCommandHandler(
    IGenericRepository<GroceryListAssignee> groceryListAssigneeRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateGroceryListAssigneeCommand>
{
    public async Task<Result> Handle(UpdateGroceryListAssigneeCommand request, CancellationToken cancellationToken)
    {
        GroceryListAssignee? assignee = await groceryListAssigneeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (assignee is null)
        {
            return Result.Failure(GroceryListAssigneeErrors.NotFound);
        }

        assignee.UpdateCompletionStatus(request.IsCompleted);

        groceryListAssigneeRepository.Update(assignee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
