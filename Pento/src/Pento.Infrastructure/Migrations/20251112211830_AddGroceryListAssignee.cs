using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddGroceryListAssignee : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "grocery_list",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                household_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grocery_list", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "grocery_list_assignees",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                grocery_list_id = table.Column<Guid>(type: "uuid", nullable: false),
                household_member_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                assigned_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grocery_list_assignees", x => x.id);
                table.ForeignKey(
                    name: "fk_grocery_list_assignees_grocery_list_grocery_list_id",
                    column: x => x.grocery_list_id,
                    principalTable: "grocery_list",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_grocery_list_assignees_user_household_member_id",
                    column: x => x.household_member_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });


        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_assignees_grocery_list_id_household_member_id",
            table: "grocery_list_assignees",
            columns: ["grocery_list_id", "household_member_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_assignees_household_member_id",
            table: "grocery_list_assignees",
            column: "household_member_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "grocery_list_assignees");

        migrationBuilder.DropTable(
            name: "grocery_list");
    }
}
