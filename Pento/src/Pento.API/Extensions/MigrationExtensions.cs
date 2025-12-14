using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;
using Pento.Infrastructure.Persistence;
using Pento.Infrastructure.Persistence.Seed;

namespace Pento.API.Extensions;

internal static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
        if (!dbContext.Set<Unit>().Any())
        {
            dbContext.Set<Unit>().AddRange(UnitData.Gram,
            UnitData.Kilogram,
            UnitData.Ounce,
            UnitData.Pound,

            UnitData.Millilitre,
            UnitData.Litre,
            UnitData.TeaspoonUS,
            UnitData.TablespoonUS,
            UnitData.USFluidOunce,
            UnitData.USCup,
            UnitData.USPint,
            UnitData.USQuart,
            UnitData.USGallon,

            UnitData.Serving,
            UnitData.Piece,
            UnitData.Pair,
            UnitData.Dozen);
            dbContext.SaveChanges();
        }


    }
    public static async Task ImportFoodReferences(this IApplicationBuilder app, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!await dbContext.Set<FoodReference>().AnyAsync(cancellationToken))
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };
            using Stream stream = File.OpenRead(@"food_references.csv");
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, csvConfig);
            csv.Context.RegisterClassMap<FoodReferenceCsvMap>();

            var records = new List<FoodReference>();
            await foreach (FoodReference record in csv.GetRecordsAsync<FoodReference>(cancellationToken))
            {
                records.Add(record);
            }
            dbContext.Set<FoodReference>().AddRange(records);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        

    }
    private sealed class FoodReferenceCsvMap : ClassMap<FoodReference>
    {
        public FoodReferenceCsvMap()
        {
            Map(m => m.Id).Name("id");

            Map(m => m.Name)
                .Name("name");

            Map(m => m.FoodGroup)
                .Name("food_group");

            Map(m => m.FoodCategoryId)
                .Name("food_category_id");

            Map(m => m.Brand)
                .Name("brand");

            Map(m => m.Barcode)
                .Name("barcode");

            Map(m => m.UsdaId)
                .Name("usda_id");

            Map(m => m.TypicalShelfLifeDays_Pantry)
                .Name("typical_shelf_life_days_pantry");

            Map(m => m.TypicalShelfLifeDays_Fridge)
                .Name("typical_shelf_life_days_fridge");

            Map(m => m.TypicalShelfLifeDays_Freezer)
                .Name("typical_shelf_life_days_freezer");

            Map(m => m.ImageUrl)
                .Name("image_url");

            Map(m => m.CreatedOnUtc)
                .Name("created_on_utc");

            Map(m => m.UpdatedOnUtc)
                .Name("updated_on_utc");

            Map(m => m.AddedBy)
                .Name("added_by");

            Map(m => m.UnitType)
                .Name("unit_type");
            Map(m => m.IsDeleted)
                .Name("is_deleted");
        }
    }
}
