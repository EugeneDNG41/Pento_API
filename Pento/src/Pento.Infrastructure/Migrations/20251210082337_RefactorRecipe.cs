using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RefactorRecipe : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "giveaway_claims");

        migrationBuilder.DropTable(
            name: "giveaway_posts");

        migrationBuilder.DropColumn(
            name: "calories_per_serving",
            table: "recipes");

        migrationBuilder.DropColumn(
            name: "recipe_cook_time_minutes",
            table: "recipes");

        migrationBuilder.DropColumn(
            name: "recipe_prep_time_minutes",
            table: "recipes");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "calories_per_serving",
            table: "recipes",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "recipe_cook_time_minutes",
            table: "recipes",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "recipe_prep_time_minutes",
            table: "recipes",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "giveaway_posts",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                address = table.Column<string>(type: "text", nullable: false),
                contact_info = table.Column<string>(type: "text", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                food_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                pickup_end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                pickup_option = table.Column<int>(type: "integer", nullable: false),
                pickup_start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                title_description = table.Column<string>(type: "text", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_giveaway_posts", x => x.id);
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
                claimant_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                giveaway_post_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                message = table.Column<string>(type: "text", nullable: true),
                status = table.Column<int>(type: "integer", nullable: false)
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

        migrationBuilder.CreateIndex(
            name: "ix_giveaway_claims_claimant_id",
            table: "giveaway_claims",
            column: "claimant_id");

        migrationBuilder.CreateIndex(
            name: "ix_giveaway_claims_giveaway_post_id",
            table: "giveaway_claims",
            column: "giveaway_post_id");

        migrationBuilder.CreateIndex(
            name: "ix_giveaway_posts_user_id",
            table: "giveaway_posts",
            column: "user_id");
    }
}
