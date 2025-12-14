using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("meal_plans");

        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.HouseholdId)
            .IsRequired();

        builder.HasOne<Household>()
            .WithMany()
            .HasForeignKey(mp => mp.HouseholdId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(mp => mp.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(mp => mp.MealType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(mp => mp.ScheduledDate)
            .HasColumnName("scheduled_date")
            .IsRequired();

        builder.Property(mp => mp.Servings)
            .IsRequired();

        builder.Property(mp => mp.Notes)
            .HasMaxLength(500);

        builder.Property(mp => mp.CreatedBy)
            .IsRequired();

        builder.Property(mp => mp.CreatedOnUtc)
            .HasColumnName("created_on_utc")
            .IsRequired();

        builder.Property(mp => mp.UpdatedOnUtc)
            .HasColumnName("updated_on_utc")
            .IsRequired();
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property<uint>("Version").IsRowVersion();
    }
}
