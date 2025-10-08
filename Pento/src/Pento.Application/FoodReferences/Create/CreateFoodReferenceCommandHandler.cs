using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Users;

namespace Pento.Application.FoodReferences.Create;
internal sealed class CreateFoodReferenceCommandHandler(IFoodReferenceRepository foodReferenceRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateFoodReferenceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateFoodReferenceCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<FoodGroup>(request.FoodGroup, true, out FoodGroup foodGroup))
        {
            return Result.Failure<Guid>(FoodReferenceErrors.InvalidGroup);
        }
        DateTime utcNow = DateTime.UtcNow;

        var foodReference = FoodReference.Create(
            request.Name,
            foodGroup,
            request.Barcode,
            request.Brand,
            request.TypicalShelfLifeDays,
            request.OpenFoodFactsId,
            request.UsdaId,
            utcNow
        );

        await foodReferenceRepository.AddAsync(foodReference, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(foodReference.Id);
    }
}
