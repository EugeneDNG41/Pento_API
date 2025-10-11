using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.MealPlanItems;
using Pento.Domain.MealPlans;
using Pento.Domain.Recipes;

namespace Pento.Infrastructure.Configurations;
internal sealed class MealPlanItemConfiguration : IEntityTypeConfiguration<MealPlanItem>
{
    public void Configure(EntityTypeBuilder<MealPlanItem> builder)
    {
        builder.ToTable("meal_plan_items");

        builder.HasKey(mpi => mpi.Id);

        builder.Property(mpi => mpi.MealPlanId)
            .IsRequired();

        builder.Property(mpi => mpi.RecipeId)
            .IsRequired();

        builder.Property(mpi => mpi.Schedule)
         .HasConversion(
             v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
             v => JsonSerializer.Deserialize<List<DateTime>>(v, (JsonSerializerOptions?)null) ?? new List<DateTime>()
         )
         .HasColumnName("schedule")
         .HasColumnType("TEXT") 
         .IsRequired();

        builder.Property(mpi => mpi.MealType)
            .HasConversion<string>()
            .HasMaxLength(100)
            .HasColumnName("meal_type")
            .IsRequired();

        builder.Property(mpi => mpi.Servings)
            .IsRequired();

        builder.Property(mpi => mpi.Notes)
            .HasMaxLength(1000);

        builder.Property(mpi => mpi.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .IsRequired();

        builder.Property(mpi => mpi.UpdatedOnUtc)
            .HasColumnName("updated_on_utc")
            .IsRequired();

        builder.HasOne<MealPlan>()
            .WithMany()
            .HasForeignKey(mpi => mpi.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(mpi => mpi.RecipeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
