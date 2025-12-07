using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class NewActivities : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "created_by",
            table: "recipes",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        migrationBuilder.InsertData(
            table: "activities",
            columns: columns,
            values: new object[,]
            {
                { "COMPARTMENT_CREATE", "Creating a new compartment within a storage location to better organize your food items.", "Create Compartment" },
                { "FOOD_ITEM_DISCARD", "Discarding a food item from your storage/compartment.", "Discard Food Item" },
                { "FOOD_ITEM_INTAKE", "Adding a new food item to your storage/compartment.", "Intake Food Item" },
                { "GROCERY_LIST_CREATE", "Creating a new grocery list to keep track of items you need to buy.", "Create Grocery List" },
                { "HOUSEHOLD_JOIN", "Joining an existing household to collaborate on food management.", "Join Household" },
                { "HOUSEHOLD_MEMBER_JOINED", "A new member has joined your household to help manage food, grocery lists, and meal plans.", "Household Member Joined" },
                { "MEAL_PLAN_CANCELLED", "Cancelling a meal plan that is no longer needed.", "Cancel Meal Plan" },
                { "MEAL_PLAN_CREATE", "Creating a new meal plan to organize your meals for the week.", "Create Meal Plan" },
                { "MEAL_PLAN_FULFILLED", "Completing a meal plan by preparing the planned meals.", "Fulfill Meal Plan" },
                { "RECIPE_COOK", "Cooking a recipe using ingredients from your food items.", "Cook Recipe" },
                { "RECIPE_CREATE", "Creating a new recipe to plan your meals and manage ingredients.", "Create Recipe" },
                { "RECIPE_OTHER_COOK", "Having your recipe cooked by another user", "Recipe Cooked By Other" }
            });

        migrationBuilder.CreateIndex(
            name: "ix_recipes_created_by",
            table: "recipes",
            column: "created_by");

        migrationBuilder.AddForeignKey(
            name: "fk_recipes_user_created_by",
            table: "recipes",
            column: "created_by",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_recipes_user_created_by",
            table: "recipes");

        migrationBuilder.DropIndex(
            name: "ix_recipes_created_by",
            table: "recipes");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "COMPARTMENT_CREATE");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "FOOD_ITEM_DISCARD");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "FOOD_ITEM_INTAKE");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "GROCERY_LIST_CREATE");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "HOUSEHOLD_JOIN");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "HOUSEHOLD_MEMBER_JOINED");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "MEAL_PLAN_CANCELLED");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "MEAL_PLAN_CREATE");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "MEAL_PLAN_FULFILLED");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "RECIPE_COOK");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "RECIPE_CREATE");

        migrationBuilder.DeleteData(
            table: "activities",
            keyColumn: "code",
            keyValue: "RECIPE_OTHER_COOK");

        migrationBuilder.AlterColumn<Guid>(
            name: "created_by",
            table: "recipes",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");
    }
}
