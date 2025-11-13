using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pento.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddGroceryListItem : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "grocery_list_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                list_id = table.Column<Guid>(type: "uuid", nullable: false),
                food_ref_id = table.Column<Guid>(type: "uuid", nullable: false),
                custom_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                quantity = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                estimated_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                priority = table.Column<string>(type: "text", nullable: false),
                added_by = table.Column<Guid>(type: "uuid", nullable: false),
                created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_archived = table.Column<bool>(type: "boolean", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_grocery_list_items", x => x.id);
                table.ForeignKey(
                    name: "fk_grocery_list_items_food_references_food_ref_id",
                    column: x => x.food_ref_id,
                    principalTable: "food_references",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_grocery_list_items_grocery_list_list_id",
                    column: x => x.list_id,
                    principalTable: "grocery_list",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_grocery_list_items_units_unit_id",
                    column: x => x.unit_id,
                    principalTable: "units",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "fk_grocery_list_items_user_added_by",
                    column: x => x.added_by,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_items_added_by",
            table: "grocery_list_items",
            column: "added_by");

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_items_food_ref_id",
            table: "grocery_list_items",
            column: "food_ref_id");

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_items_list_id_food_ref_id",
            table: "grocery_list_items",
            columns: ["list_id", "food_ref_id"]);

        migrationBuilder.CreateIndex(
            name: "ix_grocery_list_items_unit_id",
            table: "grocery_list_items",
            column: "unit_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "grocery_list_items");
    }
}
