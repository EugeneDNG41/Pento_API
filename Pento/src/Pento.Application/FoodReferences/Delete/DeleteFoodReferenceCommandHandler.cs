using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
