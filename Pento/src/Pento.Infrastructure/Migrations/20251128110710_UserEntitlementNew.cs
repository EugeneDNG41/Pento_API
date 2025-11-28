using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UserEntitlementNew : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "is_archived",
            table: "user_entitlements");

        migrationBuilder.DropColumn(
            name: "is_deleted",
            table: "user_entitlements");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "is_archived",
            table: "user_entitlements",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            table: "user_entitlements",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
