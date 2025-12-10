using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.MealPlans;
using Pento.Domain.Recipes;
using Pento.Domain.Trades;

namespace Pento.Infrastructure.Configurations;

internal sealed class FoodItemReservationConfiguration : IEntityTypeConfiguration<FoodItemReservation>
{
    public void Configure(EntityTypeBuilder<FoodItemReservation> builder)
    {
        builder.ToTable("food_item_reservations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FoodItemId)
               .IsRequired();
        builder.Property(x => x.HouseholdId)
               .IsRequired();
        builder.Property(x => x.ReservationDateUtc)
               .IsRequired();
        builder.Property(x => x.Quantity)
               .HasColumnType("decimal(10,3)")
               .IsRequired();
        builder.Property(x => x.UnitId)
               .IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(10)
               .IsRequired();
        builder.Property(x => x.ReservationFor).HasConversion<string>().HasMaxLength(10)
               .IsRequired();
        builder.UseTphMappingStrategy().HasDiscriminator(x => x.ReservationFor)
                .HasValue<FoodItemRecipeReservation>(ReservationFor.Recipe)
               .HasValue<FoodItemMealPlanReservation>(ReservationFor.MealPlan)
               .HasValue<FoodItemTradeReservation>(ReservationFor.Trade);
        builder.Property<uint>("Version").IsRowVersion();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
internal sealed class FoodItemRecipeReservationConfiguration : IEntityTypeConfiguration<FoodItemRecipeReservation>
{
    public void Configure(EntityTypeBuilder<FoodItemRecipeReservation> builder)
    {
        builder.Property(x => x.RecipeId)
               .IsRequired();
        builder.HasOne<Recipe>()
               .WithMany()
               .HasForeignKey(x => x.RecipeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class FoodItemMealPlanReservationConfiguration : IEntityTypeConfiguration<FoodItemMealPlanReservation>
{
    public void Configure(EntityTypeBuilder<FoodItemMealPlanReservation> builder)
    {
        builder.Property(x => x.MealPlanId)
               .IsRequired();
        builder.HasOne<MealPlan>()
                .WithMany()
                .HasForeignKey(x => x.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade);
    }
}
internal sealed class FoodItemDonationReservationConfiguration : IEntityTypeConfiguration<FoodItemTradeReservation>
{
    public void Configure(EntityTypeBuilder<FoodItemTradeReservation> builder)
    {
        builder.Property(x => x.TradeItemId)
               .IsRequired();
        builder.HasOne<TradeItem>()
               .WithMany()
               .HasForeignKey(x => x.TradeItemId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
