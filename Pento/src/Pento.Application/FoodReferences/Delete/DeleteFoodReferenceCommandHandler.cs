using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Delete;

internal sealed class DeleteFoodReferenceCommandHandler(
    IGenericRepository<FoodReference> foodReferenceRepository,
    IUnitOfWork unitOfWork
    )
    : ICommandHandler<DeleteFoodReferenceCommand>
{
    public async Task<Result> Handle(DeleteFoodReferenceCommand request, CancellationToken cancellationToken)
    {
        FoodReference? foodRef = await foodReferenceRepository.GetByIdAsync(request.FoodReferenceId, cancellationToken);
        if (foodRef is null)
        {
            return Result.Failure(FoodReferenceErrors.NotFound);
        }
        foodRef.Delete();
        foodReferenceRepository.Update(foodRef);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
