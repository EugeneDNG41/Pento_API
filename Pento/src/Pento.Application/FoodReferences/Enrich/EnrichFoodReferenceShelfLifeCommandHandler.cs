﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Enrich;
internal sealed class EnrichFoodReferenceShelfLifeCommandHandler(
    IFoodReferenceRepository foodReferenceRepository,
    IFoodAiEnricher foodAiEnricher,
    IUnitOfWork unitOfWork)
    : ICommandHandler<EnrichFoodReferenceShelfLifeCommand, FoodEnrichmentResult>
{
    public async Task<Result<FoodEnrichmentResult>> Handle(
        EnrichFoodReferenceShelfLifeCommand request,
        CancellationToken cancellationToken)
    {
        FoodReference? foodRef = await foodReferenceRepository.GetByIdAsync(request.FoodReferenceId, cancellationToken);
        if (foodRef is null)
        {
            return Result.Failure<FoodEnrichmentResult>(FoodReferenceErrors.NotFound(request.FoodReferenceId));
        }

        FoodEnrichmentResult enrichment = await foodAiEnricher.EnrichAsync(
            new FoodEnrichmentAsk(
                foodRef.Name,
                foodRef.FoodGroup.ToString(),
                foodRef.Notes ?? string.Empty
            ),
            cancellationToken
        );

        foodRef.UpdateShelfLifeDays(
            pantryDays: enrichment.TypicalShelfLifeDays_Pantry,
            fridgeDays: enrichment.TypicalShelfLifeDays_Fridge,
            freezerDays: enrichment.TypicalShelfLifeDays_Freezer,
            updatedAtUtc: DateTime.UtcNow
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(enrichment);
    }
}
