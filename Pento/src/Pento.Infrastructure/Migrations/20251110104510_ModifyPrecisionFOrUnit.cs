using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ModifyPrecisionFOrUnit : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "to_base_factor",
            table: "units",
            type: "numeric(10,3)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(10,2)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "to_base_factor",
            table: "units",
            type: "numeric(10,2)",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "numeric(10,3)");
    }
}
