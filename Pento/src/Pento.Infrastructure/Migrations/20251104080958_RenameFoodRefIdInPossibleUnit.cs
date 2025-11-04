using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RenameFoodRefIdInPossibleUnit : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "food_ref_id",
            table: "possible_units",
            newName: "food_reference_id");

        migrationBuilder.RenameIndex(
            name: "ix_possible_units_food_ref_id_unit_id",
            table: "possible_units",
            newName: "ix_possible_units_food_reference_id_unit_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "food_reference_id",
            table: "possible_units",
            newName: "food_ref_id");

        migrationBuilder.RenameIndex(
            name: "ix_possible_units_food_reference_id_unit_id",
            table: "possible_units",
            newName: "ix_possible_units_food_ref_id_unit_id");
    }
}
