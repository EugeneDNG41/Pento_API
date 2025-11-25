using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class NewPermission : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };
    private static readonly string[] columnsArray = new[] { "permission_code", "role_name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "permissions",
            columns: columns,
            values: ["payments:manage", "Manage payment settings and view transaction history.", "Manage Payments"]);

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: columnsArray,
            values: ["payments:manage", "Administrator"]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: columnsArray,
            keyValues: ["payments:manage", "Administrator"]);

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "payments:manage");
    }
}
