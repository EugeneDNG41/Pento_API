using Microsoft.EntityFrameworkCore;
using Pento.Domain.DietaryTags;
using Pento.Domain.Units;
using Pento.Infrastructure;

namespace Pento.API.Extensions;

internal static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
        if (!dbContext.Units.Any())
        {
            dbContext.Units.AddRange(UnitData.Gram,
            UnitData.Kilogram,
            UnitData.Milligram,
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
            UnitData.Each,
            UnitData.Pair,
            UnitData.Dozen);
            dbContext.SaveChanges();
        }
        if (!dbContext.DietaryTags.Any())
        {
            dbContext.DietaryTags.AddRange(
                DietaryTagData.ContainsSugar,
                DietaryTagData.ContainsFat,
                DietaryTagData.ContainsSaturatedFat,
                DietaryTagData.ContainsTransFat,
                DietaryTagData.ContainsCarbohydrates,
                DietaryTagData.ContainsProtein,
                DietaryTagData.ContainsFiber,
                DietaryTagData.ContainsSodium,
                DietaryTagData.ContainsOil,

                DietaryTagData.ContainsDairy,
                DietaryTagData.ContainsMilkProtein,
                DietaryTagData.ContainsLactose,
                DietaryTagData.ContainsEgg,

                DietaryTagData.ContainsGluten,
                DietaryTagData.ContainsWheat,
                DietaryTagData.ContainsBarley,
                DietaryTagData.ContainsRye,

                DietaryTagData.ContainsAlmonds,
                DietaryTagData.ContainsCashews,
                DietaryTagData.ContainsWalnuts,
                DietaryTagData.ContainsPistachios,
                DietaryTagData.ContainsHazelnuts,
                DietaryTagData.ContainsPecan,
                DietaryTagData.ContainsTreeNuts,

                DietaryTagData.ContainsPeanuts,
                DietaryTagData.ContainsSoy,

                DietaryTagData.ContainsFish,
                DietaryTagData.ContainsShellfish,
                DietaryTagData.ContainsCrustaceans,
                DietaryTagData.ContainsMollusks,

                DietaryTagData.ContainsSesame,
                DietaryTagData.ContainsMustard,
                DietaryTagData.ContainsCelery,
                DietaryTagData.ContainsLupine,

                DietaryTagData.ContainsAlcohol,
                DietaryTagData.ContainsCaffeine,
                DietaryTagData.ContainsAdditives,
                DietaryTagData.ContainsArtificialSweeteners,
                DietaryTagData.ContainsPreservatives,
                DietaryTagData.ContainsColoring,
                DietaryTagData.ContainsFlavorEnhancers
            );

            dbContext.SaveChanges();
        }

    }
}
