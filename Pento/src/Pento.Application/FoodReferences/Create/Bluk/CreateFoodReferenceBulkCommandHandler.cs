using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Create.Bluk;
internal sealed class CreateFoodReferenceBulkCommandHandler(
    IGenericRepository<FoodReference> foodReferenceRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateFoodReferenceBulkCommand>
{
    public async Task<Result> Handle(
        CreateFoodReferenceBulkCommand bulk,
        CancellationToken cancellationToken)
    {
        foreach (CreateFoodReferenceCommand cmd in bulk.Commands)
        {
            var entity = FoodReference.Create(
                cmd.Name,
                cmd.FoodGroup,
                cmd.FoodCategoryId,
                cmd.Brand,
                cmd.Barcode,
                cmd.UsdaId,
                cmd.TypicalShelfLifeDays_Pantry,
                cmd.TypicalShelfLifeDays_Fridge,
                cmd.TypicalShelfLifeDays_Freezer,
                cmd.AddedBy,
                cmd.ImageUrl,
                cmd.UnitType,
                DateTime.UtcNow
            );

            foodReferenceRepository.Add(entity);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

