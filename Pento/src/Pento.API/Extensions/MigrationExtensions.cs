using Microsoft.EntityFrameworkCore;
using Pento.Domain.Units;
using Pento.Infrastructure.Persistence;

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
}
