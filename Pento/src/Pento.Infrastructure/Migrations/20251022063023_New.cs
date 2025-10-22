using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class New : Migration
{
    private static readonly string[] columns = new[] { "food_ref_id", "unit_id" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "food_references",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                food_group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                data_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                food_category_id = table.Column<int>(type: "integer", nullable: true),
                brand = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                usda_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                published_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                typical_shelf_life_days_pantry = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                typical_shelf_life_days_fridge = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                typical_shelf_life_days_freezer = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_references", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "household",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_household", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                type = table.Column<string>(type: "text", nullable: false),
                content = table.Column<string>(type: "jsonb", nullable: false),
                processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                error = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_outbox_messages", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "possible_units",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                food_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_default = table.Column<bool>(type: "boolean", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_possible_units", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "recipe_directions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                step_number = table.Column<int>(type: "integer", nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                image_url = table.Column<string>(type: "text", nullable: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipe_directions", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "recipes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                recipe_prep_time_minutes = table.Column<int>(type: "integer", nullable: true),
                recipe_cook_time_minutes = table.Column<int>(type: "integer", nullable: true),
                notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                servings = table.Column<int>(type: "integer", nullable: true),
                difficulty_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                calories_per_serving = table.Column<int>(type: "integer", nullable: true),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                prep_time_minutes = table.Column<int>(type: "integer", nullable: false),
                cook_time_minutes = table.Column<int>(type: "integer", nullable: false),
                is_public = table.Column<bool>(type: "boolean", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipes", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "roles",
            columns: table => new
            {
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_roles", x => x.name);
            });

        migrationBuilder.CreateTable(
            name: "storages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_storages", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "units",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                to_base_factor = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_units", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                avatar_url = table.Column<string>(type: "text", nullable: true),
                email = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                first_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                last_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                identity_id = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "meal_plans",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                start_date = table.Column<DateOnly>(type: "date", nullable: false),
                end_date = table.Column<DateOnly>(type: "date", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meal_plans", x => x.id);
                table.ForeignKey(
                    name: "fk_meal_plans_household_household_id",
                    column: x => x.household_id,
                    principalTable: "household",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "compartments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                storage_id = table.Column<Guid>(type: "uuid", nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_compartments", x => x.id);
                table.ForeignKey(
                    name: "fk_compartments_storage_storage_id",
                    column: x => x.storage_id,
                    principalTable: "storages",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "recipe_ingredients",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                food_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipe_ingredients", x => x.id);
                table.ForeignKey(
                    name: "fk_recipe_ingredients_food_references_food_ref_id",
                    column: x => x.food_ref_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_recipe_ingredients_recipes_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_recipe_ingredients_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "blog_posts",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "text", nullable: false),
                content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                post_type = table.Column<int>(type: "integer", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_blog_posts", x => x.id);
                table.ForeignKey(
                    name: "fk_blog_posts_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_roles",
            columns: table => new
            {
                role_name = table.Column<string>(type: "character varying(50)", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_roles", x => new { x.role_name, x.user_id });
                table.ForeignKey(
                    name: "fk_user_roles_roles_roles_name",
                    column: x => x.role_name,
                    principalTable: "roles",
                    principalColumn: "name",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_roles_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "meal_plan_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                meal_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                meal_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                servings = table.Column<int>(type: "integer", nullable: false),
                notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                schedule = table.Column<string>(type: "TEXT", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meal_plan_items", x => x.id);
                table.ForeignKey(
                    name: "fk_meal_plan_items_meal_plans_meal_plan_id",
                    column: x => x.meal_plan_id,
                    principalTable: "meal_plans",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_meal_plan_items_recipe_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "storage_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                storage_id = table.Column<Guid>(type: "uuid", nullable: false),
                compartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                custom_name = table.Column<string>(type: "text", nullable: true),
                quantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                expiration_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_storage_items", x => x.id);
                table.ForeignKey(
                    name: "fk_storage_items_compartments_compartment_id",
                    column: x => x.compartment_id,
                    principalTable: "compartments",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_storage_items_food_references_food_ref_id",
                    column: x => x.food_ref_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_storage_items_storages_storage_id",
                    column: x => x.storage_id,
                    principalTable: "storages",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_storage_items_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "comments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                blog_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                is_moderated = table.Column<bool>(type: "boolean", nullable: false),
                moderated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_comments", x => x.id);
                table.ForeignKey(
                    name: "fk_comments_blog_posts_blog_post_id",
                    column: x => x.blog_post_id,
                    principalTable: "blog_posts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_comments_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "giveaway_posts",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                storage_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                title_description = table.Column<string>(type: "text", nullable: false),
                contact_info = table.Column<string>(type: "text", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                pickup_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                pickup_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                pickup_option = table.Column<int>(type: "integer", nullable: false),
                address = table.Column<string>(type: "text", nullable: false),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_giveaway_posts", x => x.id);
                table.ForeignKey(
                    name: "fk_giveaway_posts_storage_item_storage_item_id",
                    column: x => x.storage_item_id,
                    principalTable: "storage_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_giveaway_posts_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "giveaway_claims",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                giveaway_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                claimant_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                message = table.Column<string>(type: "text", nullable: true),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_giveaway_claims", x => x.id);
                table.ForeignKey(
                    name: "fk_giveaway_claims_giveaway_post_giveaway_post_id",
                    column: x => x.giveaway_post_id,
                    principalTable: "giveaway_posts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_giveaway_claims_user_claimant_id",
                    column: x => x.claimant_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "roles",
            column: "name",
            values: new object[]
            {
                "Administrator",
                "Basic Member",
                "Grocery Runner",
                "Household Admin",
                "Meal Planner",
                "Power Member",
                "Storage Manager"
            });

        migrationBuilder.CreateIndex(
            name: "ix_blog_posts_user_id",
            table: "blog_posts",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_comments_blog_post_id",
            table: "comments",
            column: "blog_post_id");

        migrationBuilder.CreateIndex(
            name: "ix_comments_user_id",
            table: "comments",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_compartments_storage_id",
            table: "compartments",
            column: "storage_id");

        migrationBuilder.CreateIndex(
            name: "ix_giveaway_claims_claimant_id",
            table: "giveaway_claims",
            column: "claimant_id");

        migrationBuilder.CreateIndex(
            name: "ix_giveaway_claims_giveaway_post_id",
            table: "giveaway_claims",
            column: "giveaway_post_id");

        migrationBuilder.CreateIndex(
            name: "ix_giveaway_posts_storage_item_id",
            table: "giveaway_posts",
            column: "storage_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_giveaway_posts_user_id",
            table: "giveaway_posts",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plan_items_meal_plan_id",
            table: "meal_plan_items",
            column: "meal_plan_id");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plan_items_recipe_id",
            table: "meal_plan_items",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plans_household_id",
            table: "meal_plans",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_possible_units_food_ref_id_unit_id",
            table: "possible_units",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_recipe_ingredients_food_ref_id",
            table: "recipe_ingredients",
            column: "food_ref_id");

        migrationBuilder.CreateIndex(
            name: "ix_recipe_ingredients_recipe_id",
            table: "recipe_ingredients",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_recipe_ingredients_unit_id",
            table: "recipe_ingredients",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_storage_items_compartment_id",
            table: "storage_items",
            column: "compartment_id");

        migrationBuilder.CreateIndex(
            name: "ix_storage_items_food_ref_id",
            table: "storage_items",
            column: "food_ref_id");

        migrationBuilder.CreateIndex(
            name: "ix_storage_items_storage_id",
            table: "storage_items",
            column: "storage_id");

        migrationBuilder.CreateIndex(
            name: "ix_storage_items_unit_id",
            table: "storage_items",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_user_id",
            table: "user_roles",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_identity_id",
            table: "users",
            column: "identity_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "comments");

        migrationBuilder.DropTable(
            name: "giveaway_claims");

        migrationBuilder.DropTable(
            name: "meal_plan_items");

        migrationBuilder.DropTable(
            name: "outbox_messages");

        migrationBuilder.DropTable(
            name: "possible_units");

        migrationBuilder.DropTable(
            name: "recipe_directions");

        migrationBuilder.DropTable(
            name: "recipe_ingredients");

        migrationBuilder.DropTable(
            name: "user_roles");

        migrationBuilder.DropTable(
            name: "blog_posts");

        migrationBuilder.DropTable(
            name: "giveaway_posts");

        migrationBuilder.DropTable(
            name: "meal_plans");

        migrationBuilder.DropTable(
            name: "recipes");

        migrationBuilder.DropTable(
            name: "roles");

        migrationBuilder.DropTable(
            name: "storage_items");

        migrationBuilder.DropTable(
            name: "users");

        migrationBuilder.DropTable(
            name: "household");

        migrationBuilder.DropTable(
            name: "compartments");

        migrationBuilder.DropTable(
            name: "food_references");

        migrationBuilder.DropTable(
            name: "units");

        migrationBuilder.DropTable(
            name: "storages");
    }
}
