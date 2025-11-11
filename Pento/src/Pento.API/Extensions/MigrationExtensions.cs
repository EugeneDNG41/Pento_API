using Microsoft.EntityFrameworkCore;
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
    }
}
