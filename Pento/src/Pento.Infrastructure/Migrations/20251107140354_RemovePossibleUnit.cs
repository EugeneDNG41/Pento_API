using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemovePossibleUnit : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "possible_units");
    }
    private static readonly string[] columns = new[] { "food_reference_id", "unit_id" };

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "possible_units",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                food_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_default = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_possible_units", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_possible_units_food_reference_id_unit_id",
            table: "possible_units",
            columns: columns,
            unique: true);
    }
}
