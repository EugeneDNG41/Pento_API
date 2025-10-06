using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class CreateDatabase : Migration
{
    private static readonly string[] columns = new[] { "permission_code", "role_name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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
                code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_permissions", x => x.code);
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
            name: "storage_item",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_storage_item", x => x.id);
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
                    principalTable: "storage_item",
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
            table: "permissions",
            column: "code",
            values: new object[]
            {
                "giveaways:manage",
                "giveaways:read",
                "grocery:manage",
                "grocery:read",
                "household:transferOwner",
                "household:update",
                "household:view",
                "mealplan:manage",
                "mealplan:read",
                "members:invite",
                "members:read",
                "members:remove",
                "members:update",
                "pantry:consume",
                "pantry:create",
                "pantry:discard",
                "pantry:read",
                "pantry:update",
                "permissions:manage",
                "permissions:overrides",
                "recipes:manage",
                "recipes:read",
                "storage:manage",
                "storage:read",
                "users:read",
                "users:update"
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

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: columns,
            values: new object[,]
            {
                { "giveaways:manage", "Administrator" },
                { "giveaways:manage", "Power Member" },
                { "giveaways:read", "Administrator" },
                { "giveaways:read", "Basic Member" },
                { "giveaways:read", "Power Member" },
                { "grocery:manage", "Administrator" },
                { "grocery:manage", "Grocery Runner" },
                { "grocery:manage", "Power Member" },
                { "grocery:read", "Administrator" },
                { "grocery:read", "Basic Member" },
                { "grocery:read", "Grocery Runner" },
                { "grocery:read", "Power Member" },
                { "household:transferOwner", "Administrator" },
                { "household:transferOwner", "Household Admin" },
                { "household:update", "Administrator" },
                { "household:update", "Household Admin" },
                { "household:update", "Power Member" },
                { "household:view", "Administrator" },
                { "household:view", "Basic Member" },
                { "household:view", "Household Admin" },
                { "household:view", "Power Member" },
                { "mealplan:manage", "Administrator" },
                { "mealplan:manage", "Meal Planner" },
                { "mealplan:manage", "Power Member" },
                { "mealplan:read", "Administrator" },
                { "mealplan:read", "Basic Member" },
                { "mealplan:read", "Meal Planner" },
                { "mealplan:read", "Power Member" },
                { "members:invite", "Administrator" },
                { "members:invite", "Household Admin" },
                { "members:read", "Administrator" },
                { "members:read", "Basic Member" },
                { "members:read", "Household Admin" },
                { "members:remove", "Administrator" },
                { "members:remove", "Household Admin" },
                { "members:update", "Administrator" },
                { "members:update", "Household Admin" },
                { "pantry:consume", "Administrator" },
                { "pantry:consume", "Power Member" },
                { "pantry:consume", "Storage Manager" },
                { "pantry:create", "Administrator" },
                { "pantry:create", "Power Member" },
                { "pantry:create", "Storage Manager" },
                { "pantry:discard", "Administrator" },
                { "pantry:discard", "Power Member" },
                { "pantry:discard", "Storage Manager" },
                { "pantry:read", "Administrator" },
                { "pantry:read", "Basic Member" },
                { "pantry:read", "Power Member" },
                { "pantry:read", "Storage Manager" },
                { "pantry:update", "Administrator" },
                { "pantry:update", "Power Member" },
                { "pantry:update", "Storage Manager" },
                { "permissions:manage", "Administrator" },
                { "permissions:manage", "Household Admin" },
                { "permissions:overrides", "Administrator" },
                { "permissions:overrides", "Household Admin" },
                { "recipes:manage", "Administrator" },
                { "recipes:manage", "Meal Planner" },
                { "recipes:manage", "Power Member" },
                { "recipes:read", "Administrator" },
                { "recipes:read", "Basic Member" },
                { "recipes:read", "Meal Planner" },
                { "recipes:read", "Power Member" },
                { "storage:manage", "Administrator" },
                { "storage:manage", "Power Member" },
                { "storage:manage", "Storage Manager" },
                { "storage:read", "Administrator" },
                { "storage:read", "Basic Member" },
                { "storage:read", "Power Member" },
                { "storage:read", "Storage Manager" },
                { "users:read", "Administrator" },
                { "users:update", "Administrator" }
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
            name: "ix_role_permissions_role_name",
            table: "role_permissions",
            column: "role_name");

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
            name: "outbox_messages");

        migrationBuilder.DropTable(
            name: "role_permissions");

        migrationBuilder.DropTable(
            name: "user_roles");

        migrationBuilder.DropTable(
            name: "blog_posts");

        migrationBuilder.DropTable(
            name: "giveaway_posts");

        migrationBuilder.DropTable(
            name: "permissions");

        migrationBuilder.DropTable(
            name: "roles");

        migrationBuilder.DropTable(
            name: "storage_item");

        migrationBuilder.DropTable(
            name: "users");
    }
}
