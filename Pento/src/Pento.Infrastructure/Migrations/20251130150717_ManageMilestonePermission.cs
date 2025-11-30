using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ManageMilestonePermission : Migration
{
    private static readonly string[] columns = new[] { "code", "description", "name" };
    private static readonly string[] columnsArray = new[] { "permission_code", "role_name" };

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "permissions",
            columns: columns,
            values: new object[] { "milestones:manage", "Create/update/delete user milestones and requirements.", "Manage Milestones" });

        migrationBuilder.InsertData(
            table: "role_permissions",
            columns: columnsArray,
            values: new object[] { "milestones:manage", "Administrator" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "role_permissions",
            keyColumns: columnsArray,
            keyValues: new object[] { "milestones:manage", "Administrator" });

        migrationBuilder.DeleteData(
            table: "permissions",
            keyColumn: "code",
            keyValue: "milestones:manage");
    }
}
