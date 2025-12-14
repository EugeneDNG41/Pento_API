using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Cleanup2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "fk_food_dietary_tags_food_reference_food_reference_id",
            table: "food_dietary_tags",
            column: "food_reference_id",
            principalTable: "food_references",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_food_dietary_tags_food_reference_food_reference_id",
            table: "food_dietary_tags");
    }
}
