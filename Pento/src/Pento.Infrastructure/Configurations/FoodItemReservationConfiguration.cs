using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.MealPlans;
using Pento.Domain.Recipes;

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
        builder.Property(x => x.Status)
               .IsRequired();
        builder.Property(x => x.ReservationFor)
               .IsRequired();
        builder.UseTphMappingStrategy().HasDiscriminator(x => x.ReservationFor)
                .HasValue<FoodItemRecipeReservation>(ReservationFor.Recipe)
               .HasValue<FoodItemMealPlanReservation>(ReservationFor.MealPlan)
               .HasValue<FoodItemDonationReservation>(ReservationFor.Donation);
        builder.Property<uint>("Version").IsRowVersion();
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
internal sealed class FoodItemDonationReservationConfiguration : IEntityTypeConfiguration<FoodItemDonationReservation>
{
    public void Configure(EntityTypeBuilder<FoodItemDonationReservation> builder)
    {
        builder.Property(x => x.GiveawayPostId)
               .IsRequired();
        builder.HasOne<GiveawayPost>()
               .WithMany()
               .HasForeignKey(x => x.GiveawayPostId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
