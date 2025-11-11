using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;

namespace Pento.Application.FoodReferences.Import;

internal sealed class ImportFoodReferencesCommandHandler(
    IFoodReferenceRepository foodReferenceRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<ImportFoodReferencesCommand, int>
{
    public async Task<Result<int>> Handle(ImportFoodReferencesCommand request, CancellationToken cancellationToken)
    {
        Microsoft.AspNetCore.Http.IFormFile? file = request.File;
        if (file is null || file.Length == 0)
        {
            return Result.Failure<int>(Error.Conflict("FoodReferences.EmptyFile", "Uploaded file is empty."));
        }

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        };

        int count = 0;
        using Stream stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, csvConfig);
        csv.Context.RegisterClassMap<FoodCsvMap>();

        var records = new List<FoodCsv>();
        await foreach (FoodCsv record in csv.GetRecordsAsync<FoodCsv>(cancellationToken))
        {
            records.Add(record);
        }
        records = records
            .DistinctBy(r => r.FdcId)
            .ToList();
        records = records
            .GroupBy(r => r.Description.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();
        foreach (FoodCsv f in records)
        {
            FoodGroup group = MapFoodGroup(f.FoodCategoryId);

            var food = FoodReference.Create(
                name: f.Description,
                foodGroup: group,
                foodCategoryId: f.FoodCategoryId,
                brand: null,
                barcode: null,
                usdaId: f.FdcId.ToString(CultureInfo.InvariantCulture),
                typicalShelfLifeDaysPantry: 0,
                typicalShelfLifeDaysFridge: 0,
                typicalShelfLifeDaysFreezer: 0,
                addedBy:null,
                imageUrl: null,
                unitType: UnitData.Gram.Type,
                utcNow: DateTime.UtcNow
            );

            await foodReferenceRepository.AddAsync(food, cancellationToken);
            count++;

            if (count % 500 == 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(count);
    }

    private static FoodGroup MapFoodGroup(int? categoryId)
    {
        if (categoryId == null)
        {
            return FoodGroup.MixedDishes;
        }

        return categoryId switch
        {
            1 => FoodGroup.Dairy,
            2 or 6 => FoodGroup.Condiments,
            3 => FoodGroup.MixedDishes,
            4 => FoodGroup.FatsOils,
            5 or 7 or 10 or 13 or 17 => FoodGroup.Meat,
            8 or 18 or 20 => FoodGroup.CerealGrainsPasta,
            9 or 11 => FoodGroup.FruitsVegetables,
            12 or 16 => FoodGroup.LegumesNutsSeeds,
            14 or 28 => FoodGroup.Beverages,
            15 => FoodGroup.Seafood,
            19 or 23 => FoodGroup.Confectionery,
            21 or 22 or 24 or 25 or 26 or 27 => FoodGroup.MixedDishes,
            _ => FoodGroup.MixedDishes
        };
    }

    private sealed class FoodCsv
    {
        public int FdcId { get; set; } = default!;
        public string Description { get; set; } = string.Empty;
        public int? FoodCategoryId { get; set; }= null!;
    }

    private sealed class FoodCsvMap : ClassMap<FoodCsv>
    {
        public FoodCsvMap()
        {
            Map(m => m.FdcId).Name("fdc_id");
            Map(m => m.Description).Name("description");
            Map(m => m.FoodCategoryId).Name("food_category_id");
        }
    }
}
