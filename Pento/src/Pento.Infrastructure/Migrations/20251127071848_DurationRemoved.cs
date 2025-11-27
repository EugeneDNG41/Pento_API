using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DurationRemoved : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "duration_unit",
            table: "subscription_plans");

        migrationBuilder.RenameColumn(
            name: "duration_value",
            table: "subscription_plans",
            newName: "duration_in_days");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "duration_in_days",
            table: "subscription_plans",
            newName: "duration_value");

        migrationBuilder.AddColumn<string>(
            name: "duration_unit",
            table: "subscription_plans",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);
    }
}
