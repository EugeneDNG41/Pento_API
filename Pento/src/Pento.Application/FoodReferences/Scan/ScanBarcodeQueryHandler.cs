using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.External.Barcode;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.Scan;

internal sealed class ScanBarcodeQueryHandler(IBarcodeService barcodeService, ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<ScanBarcodeQuery, FoodReferenceResponse>
{
    public async Task<Result<FoodReferenceResponse>> Handle(ScanBarcodeQuery request, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                food_group AS FoodGroup,                      
                typical_shelf_life_days_pantry AS TypicalShelfLifeDays_Pantry,
                typical_shelf_life_days_fridge AS TypicalShelfLifeDays_Fridge,
                typical_shelf_life_days_freezer AS TypicalShelfLifeDays_Freezer,
                added_by AS AddedBy,
                image_url AS ImageUrl,
                brand AS Brand,
                barcode AS Barcode,
                unit_type AS UnitType,
                created_on_utc AS CreatedAt,
                updated_on_utc AS UpdatedAt
            FROM food_references
            WHERE barcode = @Barcode
            """;
        CommandDefinition command = new(sql, new { request.Barcode }, cancellationToken: cancellationToken);
        FoodReferenceResponse? existingFoodReference = await connection.QueryFirstOrDefaultAsync<FoodReferenceResponse>(command);
        if (existingFoodReference != null) {
            return existingFoodReference;
        }
        Result<FoodReference> result = await barcodeService.FetchProductAsync(request.Barcode, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<FoodReferenceResponse>(result.Error);
        }
        FoodReference foodReference = result.Value;
        return new FoodReferenceResponse(
            Id: foodReference.Id,
            Name: foodReference.Name,
            FoodGroup: foodReference.FoodGroup.ToString(),
            Brand: foodReference.Brand,
            Barcode: foodReference.Barcode,
            TypicalShelfLifeDays_Pantry: foodReference.TypicalShelfLifeDays_Pantry,
            TypicalShelfLifeDays_Fridge: foodReference.TypicalShelfLifeDays_Fridge,
            TypicalShelfLifeDays_Freezer: foodReference.TypicalShelfLifeDays_Freezer,
            ImageUrl: foodReference.ImageUrl,
            AddedBy: foodReference.AddedBy,
            UnitType: foodReference.UnitType.ToString(),
            CreatedAt: foodReference.CreatedOnUtc,
            UpdatedAt: foodReference.UpdatedOnUtc
        );
    }
}
