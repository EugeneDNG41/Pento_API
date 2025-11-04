using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class MoreChange : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_giveaway_posts_food_item_food_item_id",
            table: "giveaway_posts");

        migrationBuilder.DropTable(
            name: "food_item");

        migrationBuilder.DropIndex(
            name: "ix_giveaway_posts_food_item_id",
            table: "giveaway_posts");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "food_item",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                compartment_id = table.Column<Guid>(type: "uuid", nullable: false),
                expiration_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                food_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                image_url = table.Column<string>(type: "text", nullable: true),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                notes = table.Column<string>(type: "text", nullable: true),
                quantity = table.Column<decimal>(type: "numeric", nullable: false),
                source_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_food_item", x => x.id);
                table.ForeignKey(
                    name: "fk_food_item_food_references_food_ref_id",
                    column: x => x.food_ref_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_food_item_units_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_giveaway_posts_food_item_id",
            table: "giveaway_posts",
            column: "food_item_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_food_ref_id",
            table: "food_item",
            column: "food_ref_id");

        migrationBuilder.CreateIndex(
            name: "ix_food_item_unit_id",
            table: "food_item",
            column: "unit_id");

        migrationBuilder.AddForeignKey(
            name: "fk_giveaway_posts_food_item_food_item_id",
            table: "giveaway_posts",
            column: "food_item_id",
            principalTable: "food_item",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
