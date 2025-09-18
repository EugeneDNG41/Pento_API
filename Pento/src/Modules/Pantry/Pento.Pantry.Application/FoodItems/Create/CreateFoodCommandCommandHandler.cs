using Pento.Common.Application.Messaging;
using Pento.Common.Domain;
using Pento.Modules.Pantry.Application.Abstractions.Data;
using Pento.Modules.Pantry.Domain.FoodItems;

namespace Pento.Modules.Pantry.Application.FoodItems.Create;

internal sealed class CreateFoodCommandCommandHandler(/*IFoodItemRepository repository, IUnitOfWork unitOfWork*/)
    : ICommandHandler<CreateFoodItemCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateFoodItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
