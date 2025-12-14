using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "activities",
            columns: table => new
            {
                code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_activities", x => x.code);
            });

        migrationBuilder.CreateTable(
            name: "dietary_tags",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_dietary_tags", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "features",
            columns: table => new
            {
                code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                default_quota = table.Column<int>(type: "integer", nullable: true),
                default_reset_period = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_features", x => x.code);
            });

        migrationBuilder.CreateTable(
            name: "food_references",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                food_group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                food_category_id = table.Column<int>(type: "integer", nullable: true),
                brand = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                usda_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                typical_shelf_life_days_pantry = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                typical_shelf_life_days_fridge = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                typical_shelf_life_days_freezer = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                added_by = table.Column<Guid>(type: "uuid", nullable: true),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                unit_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_references", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "grocery_list",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grocery_list", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "households",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                invite_code = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                invite_code_expiration_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_households", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "milestones",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                icon_url = table.Column<string>(type: "text", nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_milestones", x => x.id);
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
            name: "permissions",
            columns: table => new
            {
                code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_permissions", x => x.code);
            });

        migrationBuilder.CreateTable(
            name: "roles",
            columns: table => new
            {
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_roles", x => x.name);
            });

        migrationBuilder.CreateTable(
            name: "subscriptions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscriptions", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "units",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                to_base_factor = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_units", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "food_dietary_tags",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                dietary_tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_dietary_tags", x => x.id);
                table.ForeignKey(
                    name: "fk_food_dietary_tags_dietary_tags_dietary_tag_id",
                    column: x => x.dietary_tag_id,
                    principalTable: "dietary_tags",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_dietary_tags_food_reference_food_reference_id",
                    column: x => x.food_reference_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "storages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_storages", x => x.id);
                table.ForeignKey(
                    name: "fk_storages_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: true),
                avatar_url = table.Column<string>(type: "text", nullable: true),
                email = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                first_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                last_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                identity_id = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
                table.ForeignKey(
                    name: "fk_users_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "milestone_requirements",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                milestone_id = table.Column<Guid>(type: "uuid", nullable: false),
                activity_code = table.Column<string>(type: "character varying(50)", nullable: false),
                quota = table.Column<int>(type: "integer", nullable: false),
                within_days = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_milestone_requirements", x => x.id);
                table.ForeignKey(
                    name: "fk_milestone_requirements_activities_activity_code",
                    column: x => x.activity_code,
                    principalTable: "activities",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_milestone_requirements_milestones_milestone_id",
                    column: x => x.milestone_id,
                    principalTable: "milestones",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "role_permissions",
            columns: table => new
            {
                permission_code = table.Column<string>(type: "character varying(100)", nullable: false),
                role_name = table.Column<string>(type: "character varying(50)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_role_permissions", x => new { x.permission_code, x.role_name });
                table.ForeignKey(
                    name: "fk_role_permissions_permissions_permission_code",
                    column: x => x.permission_code,
                    principalTable: "permissions",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_role_permissions_roles_role_name",
                    column: x => x.role_name,
                    principalTable: "roles",
                    principalColumn: "name",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "subscription_features",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                feature_code = table.Column<string>(type: "character varying(50)", nullable: false),
                quota = table.Column<int>(type: "integer", nullable: true),
                reset_period = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscription_features", x => x.id);
                table.ForeignKey(
                    name: "fk_subscription_features_features_feature_code",
                    column: x => x.feature_code,
                    principalTable: "features",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_subscription_features_subscriptions_subscription_id",
                    column: x => x.subscription_id,
                    principalTable: "subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "subscription_plans",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                amount = table.Column<long>(type: "bigint", nullable: false),
                currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                duration_in_days = table.Column<int>(type: "integer", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscription_plans", x => x.id);
                table.ForeignKey(
                    name: "fk_subscription_plans_subscriptions_subscription_id",
                    column: x => x.subscription_id,
                    principalTable: "subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "compartments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                storage_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_compartments", x => x.id);
                table.ForeignKey(
                    name: "fk_compartments_household_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_compartments_storage_storage_id",
                    column: x => x.storage_id,
                    principalTable: "storages",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "device_tokens",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                platform = table.Column<string>(type: "text", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_device_tokens", x => x.id);
                table.ForeignKey(
                    name: "fk_device_tokens_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "grocery_list_assignees",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                grocery_list_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                assigned_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grocery_list_assignees", x => x.id);
                table.ForeignKey(
                    name: "fk_grocery_list_assignees_grocery_list_grocery_list_id",
                    column: x => x.grocery_list_id,
                    principalTable: "grocery_list",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_grocery_list_assignees_user_household_member_id",
                    column: x => x.household_member_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "grocery_list_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                list_id = table.Column<Guid>(type: "uuid", nullable: false),
                food_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                custom_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                quantity = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                priority = table.Column<string>(type: "text", nullable: false),
                added_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grocery_list_items", x => x.id);
                table.ForeignKey(
                    name: "fk_grocery_list_items_food_references_food_ref_id",
                    column: x => x.food_ref_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_grocery_list_items_grocery_list_list_id",
                    column: x => x.list_id,
                    principalTable: "grocery_list",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_grocery_list_items_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "fk_grocery_list_items_user_added_by",
                    column: x => x.added_by,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "meal_plans",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                meal_type = table.Column<string>(type: "text", nullable: false),
                scheduled_date = table.Column<DateOnly>(type: "date", nullable: false),
                servings = table.Column<int>(type: "integer", nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meal_plans", x => x.id);
                table.ForeignKey(
                    name: "fk_meal_plans_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_meal_plans_user_created_by",
                    column: x => x.created_by,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "notifications",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                body = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                type = table.Column<string>(type: "text", nullable: false),
                data_json = table.Column<string>(type: "jsonb", nullable: true),
                sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                read_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_notifications", x => x.id);
                table.ForeignKey(
                    name: "fk_notifications_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "recipes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                servings = table.Column<int>(type: "integer", nullable: true),
                difficulty_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                prep_time_minutes = table.Column<int>(type: "integer", nullable: false),
                cook_time_minutes = table.Column<int>(type: "integer", nullable: false),
                is_public = table.Column<bool>(type: "boolean", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipes", x => x.id);
                table.ForeignKey(
                    name: "fk_recipes_user_created_by",
                    column: x => x.created_by,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "trade_offers",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                pickup_option = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_offers", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_offers_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_offers_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "user_activities",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: true),
                activity_code = table.Column<string>(type: "character varying(50)", nullable: false),
                performed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                entity_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_activities", x => x.id);
                table.ForeignKey(
                    name: "fk_user_activities_activities_activity_code",
                    column: x => x.activity_code,
                    principalTable: "activities",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_activities_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_activities_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_milestones",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                milestone_id = table.Column<Guid>(type: "uuid", nullable: false),
                achieved_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_milestones", x => x.id);
                table.ForeignKey(
                    name: "fk_user_milestones_milestones_milestone_id",
                    column: x => x.milestone_id,
                    principalTable: "milestones",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_milestones_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "user_preferences",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                dietary_tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_preferences", x => x.id);
                table.ForeignKey(
                    name: "fk_user_preferences_dietary_tags_dietary_tag_id",
                    column: x => x.dietary_tag_id,
                    principalTable: "dietary_tags",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_preferences_users_user_id",
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
            name: "user_subscriptions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                start_date = table.Column<DateOnly>(type: "date", nullable: false),
                end_date = table.Column<DateOnly>(type: "date", nullable: true),
                paused_date = table.Column<DateOnly>(type: "date", nullable: true),
                remaining_days_after_pause = table.Column<int>(type: "integer", nullable: true),
                cancelled_date = table.Column<DateOnly>(type: "date", nullable: true),
                cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_subscriptions", x => x.id);
                table.ForeignKey(
                    name: "fk_user_subscriptions_subscriptions_subscription_id",
                    column: x => x.subscription_id,
                    principalTable: "subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_subscriptions_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "payments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                subscription_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                order_code = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                payment_link_id = table.Column<string>(type: "text", nullable: true),
                description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                provider_description = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                amount_due = table.Column<long>(type: "bigint", nullable: false),
                amount_paid = table.Column<long>(type: "bigint", nullable: false),
                currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                checkout_url = table.Column<string>(type: "text", nullable: true),
                qr_code = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_payments", x => x.id);
                table.ForeignKey(
                    name: "fk_payments_subscription_plan_subscription_plan_id",
                    column: x => x.subscription_plan_id,
                    principalTable: "subscription_plans",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_payments_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "food_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                compartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                expiration_date = table.Column<DateOnly>(type: "date", nullable: false),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                added_by = table.Column<Guid>(type: "uuid", nullable: false),
                last_modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_items", x => x.id);
                table.ForeignKey(
                    name: "fk_food_items_compartments_compartment_id",
                    column: x => x.compartment_id,
                    principalTable: "compartments",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_items_food_reference_food_reference_id",
                    column: x => x.food_reference_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_food_items_household_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_items_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_food_items_user_added_by",
                    column: x => x.added_by,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "meal_plan_recipes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                meal_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_meal_plan_recipes", x => x.id);
                table.ForeignKey(
                    name: "fk_meal_plan_recipes_meal_plans_meal_plan_id",
                    column: x => x.meal_plan_id,
                    principalTable: "meal_plans",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_meal_plan_recipes_recipe_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "recipe_dietary_tags",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                dietary_tag_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipe_dietary_tags", x => x.id);
                table.ForeignKey(
                    name: "fk_recipe_dietary_tags_dietary_tags_dietary_tag_id",
                    column: x => x.dietary_tag_id,
                    principalTable: "dietary_tags",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_recipe_dietary_tags_recipes_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
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
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipe_directions", x => x.id);
                table.ForeignKey(
                    name: "fk_recipe_directions_recipes_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
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
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
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
            name: "recipe_wishlists",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                added_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_recipe_wishlists", x => x.id);
                table.ForeignKey(
                    name: "fk_recipe_wishlists_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_recipe_wishlists_recipes_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "trade_requests",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_offer_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_requests", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_requests_households_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_requests_trade_offers_trade_offer_id",
                    column: x => x.trade_offer_id,
                    principalTable: "trade_offers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_requests_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "user_entitlements",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                feature_code = table.Column<string>(type: "character varying(50)", nullable: false),
                usage_count = table.Column<int>(type: "integer", nullable: false),
                quota = table.Column<int>(type: "integer", nullable: true),
                reset_period = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_entitlements", x => x.id);
                table.ForeignKey(
                    name: "fk_user_entitlements_features_feature_code",
                    column: x => x.feature_code,
                    principalTable: "features",
                    principalColumn: "code",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_entitlements_user_subscription_user_subscription_id",
                    column: x => x.user_subscription_id,
                    principalTable: "user_subscriptions",
                    principalColumn: "id");
                table.ForeignKey(
                    name: "fk_user_entitlements_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "food_item_logs",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_item_logs", x => x.id);
                table.ForeignKey(
                    name: "fk_food_item_logs_food_items_food_item_id",
                    column: x => x.food_item_id,
                    principalTable: "food_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_item_logs_household_household_id",
                    column: x => x.household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_item_logs_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_food_item_logs_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "food_item_reservations",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                reservation_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                quantity = table.Column<decimal>(type: "numeric(10,3)", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                reservation_for = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                meal_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_item_reservations", x => x.id);
                table.ForeignKey(
                    name: "fk_food_item_reservations_food_items_food_item_id",
                    column: x => x.food_item_id,
                    principalTable: "food_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_food_item_reservations_meal_plan_meal_plan_id",
                    column: x => x.meal_plan_id,
                    principalTable: "meal_plans",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_item_reservations_recipe_recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "trade_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                from = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                offer_id = table.Column<Guid>(type: "uuid", nullable: true),
                request_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_items", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_items_food_items_food_item_id",
                    column: x => x.food_item_id,
                    principalTable: "food_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_items_trade_offer_offer_id",
                    column: x => x.offer_id,
                    principalTable: "trade_offers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_items_trade_request_request_id",
                    column: x => x.request_id,
                    principalTable: "trade_requests",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_items_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "trade_sessions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_offer_id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                offer_household_id = table.Column<Guid>(type: "uuid", nullable: false),
                request_household_id = table.Column<Guid>(type: "uuid", nullable: false),
                confirmed_by_offer_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                confirmed_by_request_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                started_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_sessions", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_sessions_households_offer_household_id",
                    column: x => x.offer_household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_sessions_households_request_household_id",
                    column: x => x.request_household_id,
                    principalTable: "households",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_sessions_trade_offers_trade_offer_id",
                    column: x => x.trade_offer_id,
                    principalTable: "trade_offers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_sessions_trade_requests_trade_request_id",
                    column: x => x.trade_request_id,
                    principalTable: "trade_requests",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_sessions_user_confirmed_by_offer_user_id",
                    column: x => x.confirmed_by_offer_user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_sessions_user_confirmed_by_request_user_id",
                    column: x => x.confirmed_by_request_user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "trade_session_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                from = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                session_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_session_items", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_session_items_food_items_food_item_id",
                    column: x => x.food_item_id,
                    principalTable: "food_items",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_trade_session_items_trade_sessions_session_id",
                    column: x => x.session_id,
                    principalTable: "trade_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_session_items_unit_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "trade_session_messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                trade_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                message_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_trade_session_messages", x => x.id);
                table.ForeignKey(
                    name: "fk_trade_session_messages_trade_sessions_trade_session_id",
                    column: x => x.trade_session_id,
                    principalTable: "trade_sessions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_trade_session_messages_user_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.InsertData(
            table: "activities",
            columns: columns,
            values: new object[,]
            {
                { "COMPARTMENT_CREATE", "Creating a new compartment within a storage location to better organize your food items.", "Create Compartment" },
                { "FOOD_ITEM_CONSUME", "Consuming a food item from your storage/compartment.", "Consume Food Item" },
                { "FOOD_ITEM_DISCARD", "Discarding a food item from your storage/compartment.", "Discard Food Item" },
                { "FOOD_ITEM_INTAKE", "Adding a new food item to your storage/compartment.", "Intake Food Item" },
                { "FOOD_ITEM_TRADE_AWAY", "Giving a food item to another household through trade.", "Trade Away Food Item" },
                { "FOOD_ITEM_TRADE_IN", "Receiving a food item from another household through trade.", "Trade In Food Item" },
                { "GROCERY_LIST_CREATE", "Creating a new grocery list to keep track of items you need to buy.", "Create Grocery List" },
                { "HOUSEHOLD_CREATE", "Creating a new household to manage your food, grocery lists, and meal plans with others.", "Create Household" },
                { "HOUSEHOLD_JOIN", "Joining an existing household to collaborate on food management.", "Join Household" },
                { "HOUSEHOLD_MEMBER_JOINED", "A new member has joined your household to help manage food, grocery lists, and meal plans.", "Household Member Joined" },
                { "MEAL_PLAN_CANCELLED", "Cancelling a meal plan that is no longer needed.", "Cancel Meal Plan" },
                { "MEAL_PLAN_CREATE", "Creating a new meal plan to organize your meals for the week.", "Create Meal Plan" },
                { "MEAL_PLAN_FULFILLED", "Completing a meal plan by preparing the planned meals.", "Fulfill Meal Plan" },
                { "RECIPE_COOK", "Cooking a recipe using ingredients from your food items.", "Cook Recipe" },
                { "RECIPE_CREATE", "Creating a new recipe to plan your meals and manage ingredients.", "Create Recipe" },
                { "RECIPE_OTHER_COOK", "Having your recipe cooked by another user", "Recipe Cooked By Other" },
                { "STORAGE_CREATE", "Creating a new storage location to store your food items.", "Create Storage" }
            });

        migrationBuilder.InsertData(
            table: "features",
            columns: ["code", "default_quota", "default_reset_period", "description", "name"],
            values: new object[,]
            {
                { "AI_CHEF", null, null, "Generate personalized recipes.", "AI Chef" },
                { "GROCERY_MAP", null, null, "Show grocery options nearby on google map.", "Grocery Map" },
                { "IMAGE_RECOGNITION", 5, "Day", "Detect food items from images.", "Image Scanning" },
                { "OCR", 5, "Day", "Automatically extract food item names from photographed receipts.", "Receipt Scanning" }
            });

        migrationBuilder.InsertData(
            table: "permissions",
            columns: ["code", "description", "name"],
            values: new object[,]
            {
                { "compartments:create", "Create compartments within a storage (shelves, bins, drawers).", "Add Compartments" },
                { "compartments:delete", "Delete compartments. Usually blocked if they still contain items.", "Delete Compartments" },
                { "compartments:read", "View compartments/shelves within a storage. Read-only.", "View Compartments" },
                { "compartments:update", "Rename/reorder compartments and edit their attributes.", "Update Compartments" },
                { "fooditems:create", "Add new items to inventory.", "Add Food Items" },
                { "fooditems:delete", "Delete/remove items.", "Delete Food Items" },
                { "fooditems:read", "View inventory items, quantities, and expirations. Read-only.", "View Food Items" },
                { "fooditems:update", "Edit item details and adjust quantities (consume/waste/donate).", "Update Food Items" },
                { "foodreferences:manage", "Manage the authoritative food reference catalog.", "Manage Food References" },
                { "giveaways:create", "Create giveaway posts for surplus food items.", "Create Giveaways" },
                { "giveaways:delete", "Delete giveaway posts you created.", "Delete Giveaways" },
                { "giveaways:manage", "Create/update/approve/close giveaway posts and moderate entries.", "Manage Giveaways" },
                { "giveaways:read", "View giveaway posts and details. Read-only.", "View Giveaways" },
                { "giveaways:update", "Edit giveaway posts you created.", "Update Giveaways" },
                { "groceries:create", "Create grocery lists and add items to lists.", "Add Groceries" },
                { "groceries:delete", "Delete grocery lists and/or list items.", "Delete Groceries" },
                { "groceries:read", "View grocery lists and list items. Read-only.", "View Groceries" },
                { "groceries:update", "Edit grocery lists and list items.", "Update Groceries" },
                { "household:read", "View the current household’s profile, settings, and membership. Read-only.", "View Household" },
                { "household:update", "Update household name and invite code.", "Manage Household" },
                { "households:manage", "Create/update/merge/archive households at the system level.", "Manage Households" },
                { "households:read", "View all households across the system. Read-only.", "View Households" },
                { "mealplans:create", "Create meal plans and add meals/recipes to the schedule.", "Add Meal Plans" },
                { "mealplans:delete", "Delete meal plans.", "Delete Meal Plans" },
                { "mealplans:read", "View meal plans and scheduled recipes. Read-only.", "View Meal Plans" },
                { "mealplans:update", "Modify meal plans.", "Update Meal Plans" },
                { "members:delete", "Remove/kick members from the household and revoke their access.", "Remove Members" },
                { "members:update", "Change member roles within the household.", "Manage Members" },
                { "milestones:manage", "Create/update/delete user milestones and requirements.", "Manage Milestones" },
                { "payments:manage", "Manage payment settings and view transaction history.", "Manage Payments" },
                { "permissions:read", "View the catalog of permissions and their descriptions. Read-only.", "View Permissions" },
                { "recipes:create", "Create new recipes.", "Add Recipes" },
                { "recipes:delete", "Delete your own recipes.", "Delete Recipes" },
                { "recipes:manage", "Create/update/delete recipes and moderate community submissions.", "Manage Recipes" },
                { "recipes:read", "View recipes and their details. Read-only.", "View Recipes" },
                { "recipes:update", "edit your own recipes.", "Update Recipes" },
                { "roles:manage", "Create/update/delete roles and assign/unassign permissions to roles.", "Manage Roles" },
                { "roles:read", "View roles and their permission mappings. Read-only.", "View Roles" },
                { "storages:create", "Create new storage locations under the household.", "Add Storages" },
                { "storages:delete", "Delete a storage. Typically requires it to be empty; irreversible.", "Delete Storage" },
                { "storages:read", "List and view all storages (pantry, fridge, etc.) and their attributes. Read-only.", "View Storages" },
                { "storages:update", "Rename storages and modify their attributes.", "Update Storages" },
                { "subscriptions:manage", "Manage subscriptions and user subscriptions.", "Manage Subscriptions" },
                { "user:general", "Basic user access and functionality.", "User General" },
                { "users:delete", "Delete/deactivate user accounts according to policy.", "Delete Users" },
                { "users:manage", "Create/update users or change status (e.g., lock/enable). Excludes hard delete.", "Manage Users" },
                { "users:read", "View user accounts and basic profile/usage data. Read-only.", "View Users" }
            });

        migrationBuilder.InsertData(
            table: "roles",
            columns: ["name", "type"],
            values: new object[,]
            {
                { "Administrator", "Administrative" },
                { "Household Head", "Household" },
                { "Household Member", "Household" }
            });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: ["permission_code", "role_name"],
            values: new object[,]
            {
                { "compartments:create", "Household Head" },
                { "compartments:create", "Household Member" },
                { "compartments:delete", "Household Head" },
                { "compartments:delete", "Household Member" },
                { "compartments:read", "Household Head" },
                { "compartments:read", "Household Member" },
                { "compartments:update", "Household Head" },
                { "compartments:update", "Household Member" },
                { "fooditems:create", "Household Head" },
                { "fooditems:create", "Household Member" },
                { "fooditems:delete", "Household Head" },
                { "fooditems:delete", "Household Member" },
                { "fooditems:read", "Household Head" },
                { "fooditems:read", "Household Member" },
                { "fooditems:update", "Household Head" },
                { "fooditems:update", "Household Member" },
                { "foodreferences:manage", "Administrator" },
                { "giveaways:create", "Household Head" },
                { "giveaways:create", "Household Member" },
                { "giveaways:delete", "Household Head" },
                { "giveaways:delete", "Household Member" },
                { "giveaways:manage", "Administrator" },
                { "giveaways:update", "Household Head" },
                { "giveaways:update", "Household Member" },
                { "groceries:create", "Household Head" },
                { "groceries:create", "Household Member" },
                { "groceries:delete", "Household Head" },
                { "groceries:delete", "Household Member" },
                { "groceries:read", "Household Head" },
                { "groceries:read", "Household Member" },
                { "groceries:update", "Household Head" },
                { "groceries:update", "Household Member" },
                { "household:read", "Household Head" },
                { "household:read", "Household Member" },
                { "household:update", "Household Head" },
                { "household:update", "Household Member" },
                { "households:manage", "Administrator" },
                { "households:read", "Administrator" },
                { "mealplans:create", "Household Head" },
                { "mealplans:create", "Household Member" },
                { "mealplans:delete", "Household Head" },
                { "mealplans:delete", "Household Member" },
                { "mealplans:read", "Household Head" },
                { "mealplans:read", "Household Member" },
                { "mealplans:update", "Household Head" },
                { "mealplans:update", "Household Member" },
                { "members:delete", "Household Head" },
                { "members:update", "Household Head" },
                { "milestones:manage", "Administrator" },
                { "payments:manage", "Administrator" },
                { "permissions:read", "Administrator" },
                { "recipes:manage", "Administrator" },
                { "roles:manage", "Administrator" },
                { "roles:read", "Administrator" },
                { "storages:create", "Household Head" },
                { "storages:create", "Household Member" },
                { "storages:delete", "Household Head" },
                { "storages:delete", "Household Member" },
                { "storages:read", "Household Head" },
                { "storages:read", "Household Member" },
                { "storages:update", "Household Head" },
                { "storages:update", "Household Member" },
                { "subscriptions:manage", "Administrator" },
                { "users:delete", "Administrator" },
                { "users:manage", "Administrator" },
                { "users:read", "Administrator" }
            });

        migrationBuilder.CreateIndex(
            name: "ix_compartments_household_id",
            table: "compartments",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_compartments_storage_id",
            table: "compartments",
            column: "storage_id");

        migrationBuilder.CreateIndex(
            name: "ix_device_tokens_user_id",
            table: "device_tokens",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ux_device_tokens_token",
            table: "device_tokens",
            column: "token",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_food_dietary_tags_dietary_tag_id",
            table: "food_dietary_tags",
            column: "dietary_tag_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_dietary_tags_food_reference_id_dietary_tag_id",
            table: "food_dietary_tags",
            columns: ["food_reference_id", "dietary_tag_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_food_item_logs_food_item_id",
            table: "food_item_logs",
            column: "food_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_logs_household_id",
            table: "food_item_logs",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_logs_unit_id",
            table: "food_item_logs",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_logs_user_id",
            table: "food_item_logs",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_reservations_food_item_id",
            table: "food_item_reservations",
            column: "food_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_reservations_meal_plan_id",
            table: "food_item_reservations",
            column: "meal_plan_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_reservations_recipe_id",
            table: "food_item_reservations",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_added_by",
            table: "food_items",
            column: "added_by");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_compartment_id",
            table: "food_items",
            column: "compartment_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_food_reference_id",
            table: "food_items",
            column: "food_reference_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_household_id",
            table: "food_items",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_items_unit_id",
            table: "food_items",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_assignees_grocery_list_id_household_member_id",
            table: "grocery_list_assignees",
            columns: ["grocery_list_id", "household_member_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_assignees_household_member_id",
            table: "grocery_list_assignees",
            column: "household_member_id");

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_items_added_by",
            table: "grocery_list_items",
            column: "added_by");

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_items_food_ref_id",
            table: "grocery_list_items",
            column: "food_ref_id");

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_items_list_id_food_ref_id",
            table: "grocery_list_items",
            columns: ["list_id", "food_ref_id"]);

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_items_unit_id",
            table: "grocery_list_items",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plan_recipes_meal_plan_id_recipe_id",
            table: "meal_plan_recipes",
            columns: ["meal_plan_id", "recipe_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_meal_plan_recipes_recipe_id",
            table: "meal_plan_recipes",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plans_created_by",
            table: "meal_plans",
            column: "created_by");

        migrationBuilder.CreateIndex(
            name: "ix_meal_plans_household_id",
            table: "meal_plans",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_milestone_requirements_activity_code",
            table: "milestone_requirements",
            column: "activity_code");

        migrationBuilder.CreateIndex(
            name: "ix_milestone_requirements_milestone_id",
            table: "milestone_requirements",
            column: "milestone_id");

        migrationBuilder.CreateIndex(
            name: "ix_notifications_user_id",
            table: "notifications",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_payments_subscription_plan_id",
            table: "payments",
            column: "subscription_plan_id");

        migrationBuilder.CreateIndex(
            name: "ix_payments_user_id",
            table: "payments",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_recipe_dietary_tags_dietary_tag_id",
            table: "recipe_dietary_tags",
            column: "dietary_tag_id");

        migrationBuilder.CreateIndex(
            name: "ix_recipe_dietary_tags_recipe_id_dietary_tag_id",
            table: "recipe_dietary_tags",
            columns: ["recipe_id", "dietary_tag_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_recipe_directions_recipe_id",
            table: "recipe_directions",
            column: "recipe_id");

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
            name: "ix_recipe_wishlists_household_id_recipe_id",
            table: "recipe_wishlists",
            columns: ["household_id", "recipe_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_recipe_wishlists_recipe_id",
            table: "recipe_wishlists",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "ix_recipes_created_by",
            table: "recipes",
            column: "created_by");

        migrationBuilder.CreateIndex(
            name: "ix_role_permissions_role_name",
            table: "role_permissions",
            column: "role_name");

        migrationBuilder.CreateIndex(
            name: "ix_storages_household_id",
            table: "storages",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_subscription_features_feature_code",
            table: "subscription_features",
            column: "feature_code");

        migrationBuilder.CreateIndex(
            name: "ix_subscription_features_subscription_id",
            table: "subscription_features",
            column: "subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_subscription_plans_subscription_id",
            table: "subscription_plans",
            column: "subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_food_item_id",
            table: "trade_items",
            column: "food_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_offer_id",
            table: "trade_items",
            column: "offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_request_id",
            table: "trade_items",
            column: "request_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_items_unit_id",
            table: "trade_items",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_offers_household_id",
            table: "trade_offers",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_offers_user_id",
            table: "trade_offers",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_household_id",
            table: "trade_requests",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_trade_offer_id",
            table: "trade_requests",
            column: "trade_offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_requests_user_id",
            table: "trade_requests",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_items_food_item_id",
            table: "trade_session_items",
            column: "food_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_items_session_id",
            table: "trade_session_items",
            column: "session_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_items_unit_id",
            table: "trade_session_items",
            column: "unit_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_messages_trade_session_id",
            table: "trade_session_messages",
            column: "trade_session_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_session_messages_user_id",
            table: "trade_session_messages",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_confirmed_by_offer_user_id",
            table: "trade_sessions",
            column: "confirmed_by_offer_user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_confirmed_by_request_user_id",
            table: "trade_sessions",
            column: "confirmed_by_request_user_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_offer_household_id",
            table: "trade_sessions",
            column: "offer_household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_request_household_id",
            table: "trade_sessions",
            column: "request_household_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_trade_offer_id",
            table: "trade_sessions",
            column: "trade_offer_id");

        migrationBuilder.CreateIndex(
            name: "ix_trade_sessions_trade_request_id",
            table: "trade_sessions",
            column: "trade_request_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_activities_activity_code",
            table: "user_activities",
            column: "activity_code");

        migrationBuilder.CreateIndex(
            name: "ix_user_activities_household_id",
            table: "user_activities",
            column: "household_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_activities_user_id",
            table: "user_activities",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_entitlements_feature_code",
            table: "user_entitlements",
            column: "feature_code");

        migrationBuilder.CreateIndex(
            name: "ix_user_entitlements_user_id",
            table: "user_entitlements",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_entitlements_user_subscription_id",
            table: "user_entitlements",
            column: "user_subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_milestones_milestone_id",
            table: "user_milestones",
            column: "milestone_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_milestones_user_id",
            table: "user_milestones",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_preferences_dietary_tag_id",
            table: "user_preferences",
            column: "dietary_tag_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_preferences_user_id_dietary_tag_id",
            table: "user_preferences",
            columns: ["user_id", "dietary_tag_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_user_roles_user_id",
            table: "user_roles",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_subscriptions_subscription_id",
            table: "user_subscriptions",
            column: "subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_subscriptions_user_id",
            table: "user_subscriptions",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_household_id",
            table: "users",
            column: "household_id");

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
            name: "device_tokens");

        migrationBuilder.DropTable(
            name: "food_dietary_tags");

        migrationBuilder.DropTable(
            name: "food_item_logs");

        migrationBuilder.DropTable(
            name: "food_item_reservations");

        migrationBuilder.DropTable(
            name: "grocery_list_assignees");

        migrationBuilder.DropTable(
            name: "grocery_list_items");

        migrationBuilder.DropTable(
            name: "meal_plan_recipes");

        migrationBuilder.DropTable(
            name: "milestone_requirements");

        migrationBuilder.DropTable(
            name: "notifications");

        migrationBuilder.DropTable(
            name: "outbox_messages");

        migrationBuilder.DropTable(
            name: "payments");

        migrationBuilder.DropTable(
            name: "recipe_dietary_tags");

        migrationBuilder.DropTable(
            name: "recipe_directions");

        migrationBuilder.DropTable(
            name: "recipe_ingredients");

        migrationBuilder.DropTable(
            name: "recipe_wishlists");

        migrationBuilder.DropTable(
            name: "role_permissions");

        migrationBuilder.DropTable(
            name: "subscription_features");

        migrationBuilder.DropTable(
            name: "trade_items");

        migrationBuilder.DropTable(
            name: "trade_session_items");

        migrationBuilder.DropTable(
            name: "trade_session_messages");

        migrationBuilder.DropTable(
            name: "user_activities");

        migrationBuilder.DropTable(
            name: "user_entitlements");

        migrationBuilder.DropTable(
            name: "user_milestones");

        migrationBuilder.DropTable(
            name: "user_preferences");

        migrationBuilder.DropTable(
            name: "user_roles");

        migrationBuilder.DropTable(
            name: "grocery_list");

        migrationBuilder.DropTable(
            name: "meal_plans");

        migrationBuilder.DropTable(
            name: "subscription_plans");

        migrationBuilder.DropTable(
            name: "recipes");

        migrationBuilder.DropTable(
            name: "permissions");

        migrationBuilder.DropTable(
            name: "food_items");

        migrationBuilder.DropTable(
            name: "trade_sessions");

        migrationBuilder.DropTable(
            name: "activities");

        migrationBuilder.DropTable(
            name: "features");

        migrationBuilder.DropTable(
            name: "user_subscriptions");

        migrationBuilder.DropTable(
            name: "milestones");

        migrationBuilder.DropTable(
            name: "dietary_tags");

        migrationBuilder.DropTable(
            name: "roles");

        migrationBuilder.DropTable(
            name: "compartments");

        migrationBuilder.DropTable(
            name: "food_references");

        migrationBuilder.DropTable(
            name: "units");

        migrationBuilder.DropTable(
            name: "trade_requests");

        migrationBuilder.DropTable(
            name: "subscriptions");

        migrationBuilder.DropTable(
            name: "storages");

        migrationBuilder.DropTable(
            name: "trade_offers");

        migrationBuilder.DropTable(
            name: "users");

        migrationBuilder.DropTable(
            name: "households");
    }
}
