using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class NewAdminPermission : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };
    private static readonly string[] columnsArray = new[] { "permission_code", "role_name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "permissions",
            columns: columns,
            values: new object[] { "subscriptions:manage", "Manage subscriptions and user subscriptions.", "Manage Subscriptions" });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: columnsArray,
            values: new object[] { "subscriptions:manage", "Administrator" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: columnsArray,
            keyValues: new object[] { "subscriptions:manage", "Administrator" });

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "subscriptions:manage");
    }
}
