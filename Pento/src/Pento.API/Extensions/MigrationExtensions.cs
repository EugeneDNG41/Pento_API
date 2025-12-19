using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Pento.Domain.DietaryTags;
using Pento.Domain.FoodDietaryTags;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;
using Pento.Infrastructure.Persistence;
using Pento.Infrastructure.Persistence.Seed;

namespace Pento.API.Extensions;

internal static class MigrationExtensions
{
    public async static Task ApplyMigrations(this IApplicationBuilder app, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        DataSeeder dataSeeder =
            scope.ServiceProvider.GetRequiredService<DataSeeder>();
        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync(default);
        if (!await dbContext.Set<Unit>().AnyAsync(default))
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
            await dbContext.SaveChangesAsync(default);


        }
        if (!await dbContext.Set<DietaryTag>().AnyAsync(default))
        {
            dbContext.Set<DietaryTag>().AddRange(
                DietaryTagData.ContainsAddedSugar,
                DietaryTagData.ContainsGluten,
                DietaryTagData.ContainsEgg,
                DietaryTagData.ContainsPeanuts,
                DietaryTagData.ContainsTreeNuts,
                DietaryTagData.ContainsSoy,
                DietaryTagData.ContainsAlcohol,
                DietaryTagData.ContainsCaffeine,

                DietaryTagData.HighlyProcessed
            );


            await dbContext.SaveChangesAsync(default);
        }
        await dataSeeder.SeedAdminAsync(cancellationToken);
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
