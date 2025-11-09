using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateFoodRef : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "notes",
            table: "food_references");

        migrationBuilder.AddColumn<string>(
            name: "unit_type",
            table: "food_references",
            type: "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "unit_type",
            table: "food_references");

        migrationBuilder.AddColumn<string>(
            name: "notes",
            table: "food_references",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);
    }
}
